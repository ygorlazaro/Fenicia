import { CAlert, CButton, CForm, CModal, CModalBody, CModalFooter, CModalHeader, CModalTitle } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { FeniciaInput } from "../../../components/fenicia/fenicia-input";

const PositionModal = ({ visible, onClose, onSave, position, loading }) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        name: ""
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (position) {
            setFormData({
                name: position.name || ""
            });
        } else {
            setFormData({
                name: ""
            });
        }
        setError(null);
    }, [position, visible]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setError(null);

        if (!formData.name) {
            setError(t("common.requiredField"));
            return;
        }

        onSave(formData);
    };

    return (
        <CModal visible={visible} onClose={onClose}>
            <CModalHeader>
                <CModalTitle>{position ? t("positions.edit") : t("positions.new")}</CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSubmit}>
                <CModalBody>
                    {error && (
                        <CAlert color="danger" dismissible>
                            {error}
                        </CAlert>
                    )}

                    <div className="mb-3">
                        <FeniciaInput label="positions.name" id="name" value={formData.name} onChange={handleInputChange} required={true} />
                    </div>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={onClose} disabled={loading}>
                        {t("common.cancel")}
                    </CButton>
                    <CButton color="primary" type="submit" disabled={loading}>
                        {loading ? t("common.saving") : t("common.save")}
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default PositionModal;
