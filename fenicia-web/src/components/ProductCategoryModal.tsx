import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProductCategoryModal = ({
    visible,
    onClose,
    onSave,
    category,
    loading
}) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        name: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (category) {
            setFormData({
                name: category.name || ''
            });
        } else {
            setFormData({
                name: ''
            });
        }
        setError(null);
    }, [category, visible]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setError(null);

        if (!formData.name) {
            setError(t('categories.requiredFields'));
            return;
        }

        onSave(formData);
    };

    return (
        <CModal
            visible={visible}
            onClose={onClose}
        >
            <CModalHeader>
                <CModalTitle>
                    {category ? t('categories.edit') : t('categories.new')}
                </CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSubmit}>
                <CModalBody>
                    {error && (
                        <CAlert color="danger" dismissible>
                            {error}
                        </CAlert>
                    )}

                    <div className="mb-3">
                        <CFormLabel htmlFor="name">{t('categories.name')} *</CFormLabel>
                        <CFormInput
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleInputChange}
                            required
                        />
                    </div>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={onClose} disabled={loading}>
                        {t('common.cancel')}
                    </CButton>
                    <CButton
                        color="primary"
                        type="submit"
                        disabled={loading}
                    >
                        {loading ? t('common.saving') : t('common.save')}
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default ProductCategoryModal;
