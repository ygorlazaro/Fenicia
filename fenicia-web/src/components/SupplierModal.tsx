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
import BasicSupplierClient from '../services/basic-crud-clients';
import { fetchAddressByCep } from '../services/cep-client';

const supplierClient = new BasicSupplierClient();

const SupplierModal = ({
    visible,
    onClose,
    onSave,
    supplier,
    loading
}) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        phoneNumber: '',
        document: '',
        stateId: '',
        street: '',
        number: '',
        neighborhood: '',
        city: '',
        complement: '',
        zipCode: ''
    });
    const [states, setStates] = useState([]);
    const [loadingOptions, setLoadingOptions] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (visible) {
            loadOptions();
        }
    }, [visible]);

    useEffect(() => {
        if (supplier) {
            setFormData({
                name: supplier.name || '',
                email: supplier.email || '',
                phoneNumber: supplier.phoneNumber || '',
                document: supplier.document || '',
                stateId: supplier.stateId || '',
                street: supplier.street || '',
                number: supplier.number || '',
                neighborhood: supplier.neighborhood || '',
                city: supplier.city || '',
                complement: supplier.complement || '',
                zipCode: supplier.zipCode || ''
            });
        } else {
            setFormData({
                name: '',
                email: '',
                phoneNumber: '',
                document: '',
                stateId: '',
                street: '',
                number: '',
                neighborhood: '',
                city: '',
                complement: '',
                zipCode: ''
            });
        }
        setError(null);
    }, [supplier, visible]);

    const loadOptions = async () => {
        try {
            setLoadingOptions(true);
            const statesData = await supplierClient.getStates();
            setStates(statesData || []);
        } catch (err) {
            console.error('Failed to load states:', err);
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

    const handleCepBlur = async (e) => {
        const { name, value } = e.target;
        const cleanCep = value.replace(/\D/g, '');

        if (cleanCep.length === 8) {
            const address = await fetchAddressByCep(cleanCep);
            if (address) {
                const stateMatch = states.find(s => s.uf === address.state);
                setFormData(prev => ({
                    ...prev,
                    [name]: address.cep,
                    stateId: stateMatch?.id || prev.stateId,
                    city: address.city,
                    neighborhood: address.neighborhood,
                    street: address.street,
                    complement: address.complement || prev.complement || ''
                }));
            }
        }
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setError(null);

        if (!formData.name || !formData.email) {
            setError(t('suppliers.requiredFields'));
            return;
        }

        onSave(formData);
    };

    return (
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {supplier ? t('suppliers.edit') : t('suppliers.new')}
                </CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSubmit}>
                <CModalBody>
                    {error && (
                        <CAlert color="danger" dismissible>
                            {error}
                        </CAlert>
                    )}

                    <CRow>
                        <CCol md={8}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="name">{t('suppliers.name')} *</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="name"
                                    name="name"
                                    value={formData.name}
                                    onChange={handleInputChange}
                                    required
                                />
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="document">{t('suppliers.document')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="document"
                                    name="document"
                                    value={formData.document}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <CFormLabel htmlFor="email">{t('suppliers.email')} *</CFormLabel>
                        <CFormInput
                            type="email"
                            id="email"
                            name="email"
                            value={formData.email}
                            onChange={handleInputChange}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="phoneNumber">{t('suppliers.phone')}</CFormLabel>
                        <CFormInput
                            type="tel"
                            id="phoneNumber"
                            name="phoneNumber"
                            value={formData.phoneNumber}
                            onChange={handleInputChange}
                        />
                    </div>

                    <h6 className="mt-4 mb-3">{t('suppliers.address')}</h6>

                    <CRow>
                        <CCol md={3}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="zipCode">{t('suppliers.zipCode')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="zipCode"
                                    name="zipCode"
                                    value={formData.zipCode}
                                    onChange={handleInputChange}
                                    onBlur={handleCepBlur}
                                    placeholder="00000-000"
                                    maxLength={9}
                                />
                            </div>
                        </CCol>
                        <CCol md={3}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="stateId">{t('suppliers.state')}</CFormLabel>
                                <CFormSelect
                                    id="stateId"
                                    name="stateId"
                                    value={formData.stateId}
                                    onChange={handleInputChange}
                                    disabled={loadingOptions}
                                >
                                    <option value="">{t('common.select')}...</option>
                                    {states.map(state => (
                                        <option key={state.id} value={state.id}>
                                            {state.uf} - {state.name}
                                        </option>
                                    ))}
                                </CFormSelect>
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="city">{t('suppliers.city')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="city"
                                    name="city"
                                    value={formData.city}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="street">{t('suppliers.street')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="street"
                                    name="street"
                                    value={formData.street}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={2}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="number">{t('suppliers.number')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="number"
                                    name="number"
                                    value={formData.number}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="neighborhood">{t('suppliers.neighborhood')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="neighborhood"
                                    name="neighborhood"
                                    value={formData.neighborhood}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <CFormLabel htmlFor="complement">{t('suppliers.complement')}</CFormLabel>
                        <CFormInput
                            type="text"
                            id="complement"
                            name="complement"
                            value={formData.complement}
                            onChange={handleInputChange}
                        />
                    </div>
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

export default SupplierModal;
