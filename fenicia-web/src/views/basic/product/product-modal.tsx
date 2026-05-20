import { CAlert, CButton, CCol, CForm, CModal, CModalBody, CModalFooter, CModalHeader, CModalTitle, CRow } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { FeniciaInput } from "../../../components/fenicia/fenicia-input";
import { FeniciaSelect } from "../../../components/fenicia/fenicia-select";
import { BasicProductClient } from "../../../services/basic/basic-product-client";

const productClient = new BasicProductClient();

const ProductModal = ({ visible, onClose, onSave, product, loading }) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        name: "",
        costPrice: "",
        salesPrice: "",
        quantity: "",
        categoryId: "",
        supplierId: ""
    });
    const [categories, setCategories] = useState([]);
    const [suppliers, setSuppliers] = useState([]);
    const [loadingOptions, setLoadingOptions] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (visible) {
            loadOptions();
        }
    }, [visible]);

    useEffect(() => {
        if (product) {
            setFormData({
                name: product.name || "",
                costPrice: product.costPrice?.toString() || "",
                salesPrice: product.salesPrice?.toString() || "",
                quantity: product.quantity?.toString() || "0",
                categoryId: product.categoryId || "",
                supplierId: product.supplierId || ""
            });
        } else {
            setFormData({
                name: "",
                costPrice: "",
                salesPrice: "",
                quantity: "0",
                categoryId: "",
                supplierId: ""
            });
        }
        setError(null);
    }, [product, visible]);

    const loadOptions = async () => {
        try {
            setLoadingOptions(true);
            const [categoriesData, suppliersData] = await Promise.all([productClient.getProductCategories(), productClient.getSuppliers()]);
            setCategories(categoriesData || []);
            setSuppliers(suppliersData || []);
        } catch (err) {
            console.error("Failed to load options:", err);
        } finally {
            setLoadingOptions(false);
        }
    };

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

        if (!formData.name || !formData.categoryId || !formData.salesPrice) {
            setError(t("products.requiredFields"));
            return;
        }

        const payload = {
            name: formData.name,
            costPrice: formData.costPrice ? parseFloat(formData.costPrice) : null,
            salesPrice: parseFloat(formData.salesPrice),
            quantity: parseInt(formData.quantity) || 0,
            categoryId: formData.categoryId,
            supplierId: formData.supplierId || null
        };

        onSave(payload);
    };

    return (
        <CModal visible={visible} onClose={onClose} size="lg">
            <CModalHeader>
                <CModalTitle>{product ? t("products.edit") : t("products.new")}</CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSubmit}>
                <CModalBody>
                    {error && (
                        <CAlert color="danger" dismissible>
                            {error}
                        </CAlert>
                    )}

                    <div className="mb-3">
                        <FeniciaInput label={t("products.name")} id="name" value={formData.name} onChange={handleInputChange} required />
                    </div>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaSelect id="categoryId" value={formData.categoryId} onChange={handleInputChange} data={categories} label={t("products.category")} required />
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaSelect id="supplierId" value={formData.supplierId} onChange={handleInputChange} data={suppliers} label={t("products.supplier")} />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaInput label={t("products.quantity")} id="quantity" value={formData.quantity} onChange={handleInputChange} />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaInput label={t("products.costPrice")} id="costPrice" value={formData.costPrice} onChange={handleInputChange} />
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaInput label={t("products.salesPrice")} id="salesPrice" value={formData.salesPrice} onChange={handleInputChange} required />
                            </div>
                        </CCol>
                    </CRow>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={onClose} disabled={loading || loadingOptions}>
                        {t("common.cancel")}
                    </CButton>
                    <CButton color="primary" type="submit" disabled={loading || loadingOptions}>
                        {loading ? t("common.saving") : t("common.save")}
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default ProductModal;
