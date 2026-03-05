import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormSelect,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert,
    CRow,
    CCol
} from '@coreui/react';
import { BasicProductClient } from '../services/basic-crud-clients';

const productClient = new BasicProductClient();

const ProductModal = ({
    visible,
    onClose,
    onSave,
    product,
    loading
}) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        name: '',
        costPrice: '',
        salesPrice: '',
        quantity: '',
        categoryId: '',
        supplierId: ''
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
                name: product.name || '',
                costPrice: product.costPrice?.toString() || '',
                salesPrice: product.salesPrice?.toString() || '',
                quantity: product.quantity?.toString() || '0',
                categoryId: product.categoryId || '',
                supplierId: product.supplierId || ''
            });
        } else {
            setFormData({
                name: '',
                costPrice: '',
                salesPrice: '',
                quantity: '0',
                categoryId: '',
                supplierId: ''
            });
        }
        setError(null);
    }, [product, visible]);

    const loadOptions = async () => {
        try {
            setLoadingOptions(true);
            const [categoriesData, suppliersData] = await Promise.all([
                productClient.getProductCategories(),
                productClient.getSuppliers()
            ]);
            setCategories(categoriesData || []);
            setSuppliers(suppliersData || []);
        } catch (err) {
            console.error('Failed to load options:', err);
        } finally {
            setLoadingOptions(false);
        }
    };

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

        if (!formData.name || !formData.categoryId || !formData.salesPrice) {
            setError(t('products.requiredFields'));
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
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {product ? t('products.edit') : t('products.new')}
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
                        <CFormLabel htmlFor="name">{t('products.name')} *</CFormLabel>
                        <CFormInput
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleInputChange}
                            required
                        />
                    </div>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="categoryId">{t('products.category')} *</CFormLabel>
                                <CFormSelect
                                    id="categoryId"
                                    name="categoryId"
                                    value={formData.categoryId}
                                    onChange={handleInputChange}
                                    disabled={loadingOptions}
                                    required
                                >
                                    <option value="">{t('common.select')}...</option>
                                    {categories.map(cat => (
                                        <option key={cat.id} value={cat.id}>
                                            {cat.name}
                                        </option>
                                    ))}
                                </CFormSelect>
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="supplierId">{t('products.supplier')}</CFormLabel>
                                <CFormSelect
                                    id="supplierId"
                                    name="supplierId"
                                    value={formData.supplierId}
                                    onChange={handleInputChange}
                                    disabled={loadingOptions}
                                >
                                    <option value="">{t('common.select')}...</option>
                                    {suppliers.map(sup => (
                                        <option key={sup.id} value={sup.id}>
                                            {sup.name}
                                        </option>
                                    ))}
                                </CFormSelect>
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="quantity">{t('products.quantity')}</CFormLabel>
                                <CFormInput
                                    type="number"
                                    id="quantity"
                                    name="quantity"
                                    value={formData.quantity}
                                    onChange={handleInputChange}
                                    min="0"
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="costPrice">{t('products.costPrice')}</CFormLabel>
                                <CFormInput
                                    type="number"
                                    id="costPrice"
                                    name="costPrice"
                                    value={formData.costPrice}
                                    onChange={handleInputChange}
                                    step="0.01"
                                    min="0"
                                />
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="salesPrice">{t('products.salesPrice')} *</CFormLabel>
                                <CFormInput
                                    type="number"
                                    id="salesPrice"
                                    name="salesPrice"
                                    value={formData.salesPrice}
                                    onChange={handleInputChange}
                                    step="0.01"
                                    min="0"
                                    required
                                />
                            </div>
                        </CCol>
                    </CRow>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={onClose} disabled={loading || loadingOptions}>
                        {t('common.cancel')}
                    </CButton>
                    <CButton
                        color="primary"
                        type="submit"
                        disabled={loading || loadingOptions}
                    >
                        {loading ? t('common.saving') : t('common.save')}
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default ProductModal;
