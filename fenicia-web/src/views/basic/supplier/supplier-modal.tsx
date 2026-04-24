import {
    CAlert,
    CButton,
    CCol,
    CForm,
    CFormLabel,
    CFormSelect,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CRow
} from '@coreui/react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { FeniciaInput } from '../../../components/fenicia/fenicia-input';
import { BasicStateClient } from '../../../services/basic/basic-state-client';
import { fetchAddressByCep } from '../../../services/cep-client';
import { UpdateSupplierCommand } from "../../../types/basic/supplier/update-supplier-command";

const stateClient = new BasicStateClient();

const SupplierModal = ({
    visible,
    onClose,
    onSave,
    supplier,
    loading
}) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState<UpdateSupplierCommand>({
        name: '',
        email: '',
        phoneNumber: '',
        document: '',
        address: {
            stateId: '',
            street: '',
            number: '',
            neighborhood: '',
            city: '',
            complement: '',
            zipCode: ''
        },
        id: ''
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
                id: supplier.id,
                name: supplier.name || '',
                email: supplier.email || '',
                phoneNumber: supplier.phoneNumber || '',
                document: supplier.document || '',
                address: supplier.address
            });
        } else {
            setFormData({
                id: '',
                name: '',
                email: '',
                phoneNumber: '',
                document: '',
                address: {
                    stateId: '',
                    street: '',
                    number: '',
                    neighborhood: '',
                    city: '',
                    complement: '',
                    zipCode: ''
                }
            });
        }
        setError(null);
    }, [supplier, visible]);

    const loadOptions = async () => {
        try {
            setLoadingOptions(true);
            const statesData = await stateClient.getStates();
            setStates(statesData || []);
        } catch (err) {
            console.error('Failed to load states:', err);
        } finally {
            setLoadingOptions(false);
        }
    };

    const addressFields = ['zipCode', 'stateId', 'city', 'street', 'number', 'neighborhood', 'complement'];

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        if (addressFields.includes(name)) {
            setFormData(prev => ({
                ...prev,
                address: {
                    ...prev.address,
                    [name]: value
                }
            }));
        } else {
            setFormData(prev => ({
                ...prev,
                [name]: value
            }));
        }
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
                    address: {
                        ...prev.address,
                        zipCode: address.cep,
                        stateId: stateMatch?.id || prev.address?.stateId || '',
                        city: address.city || '',
                        neighborhood: address.neighborhood || '',
                        street: address.street || '',
                        complement: address.complement || prev.address?.complement || ''
                    }
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
                                <FeniciaInput
                                    id="name"
                                    label={t('suppliers.name')}
                                    value={formData.name}
                                    onChange={handleInputChange}
                                    required
                                />
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <FeniciaInput
                                    id="document"
                                    label={t('suppliers.document')}
                                    value={formData.document}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <FeniciaInput
                            type="email"
                            id="email"
                            label={`${t('suppliers.email')}`}
                            value={formData.email}
                            onChange={handleInputChange}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <FeniciaInput
                            type="tel"
                            id="phoneNumber"
                            label={`${t('suppliers.phone')}`}
                            value={formData.phoneNumber}
                            onChange={handleInputChange}
                        />
                    </div>

                    <h6 className="mt-4 mb-3">{t('suppliers.address')}</h6>

                    <CRow>
                        <CCol md={3}>
                            <div className="mb-3">
                                <FeniciaInput
                                    type="text"
                                    id="zipCode"
                                    label={t('suppliers.zipCode')}
                                    value={formData.address?.zipCode || ''}
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
                                    value={formData.address?.stateId || ''}
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
                                <FeniciaInput
                                    type="text"
                                    id="city"
                                    label={t('suppliers.city')}
                                    value={formData.address?.city || ''}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaInput
                                    type="text"
                                    id="street"
                                    label={t('suppliers.street')}
                                    value={formData.address?.street || ''}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={2}>
                            <div className="mb-3">
                                <FeniciaInput
                                    type="text"
                                    id="number"
                                    label={t('suppliers.number')}
                                    value={formData.address?.number || ''}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <FeniciaInput
                                    type="text"
                                    id="neighborhood"
                                    label={t('suppliers.neighborhood')}
                                    value={formData.address?.neighborhood || ''}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <FeniciaInput
                            type="text"
                            id="complement"
                            label={t('suppliers.complement')}
                            value={formData.address?.complement || ''}
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
