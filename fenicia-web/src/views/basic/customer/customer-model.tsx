import {
    CAlert,
    CButton,
    CCol,
    CForm,
    CFormInput,
    CFormLabel,
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
import { FeniciaSelect } from '../../../components/fenicia/fenicia-select';
import BasicCustomerClient from '../../../services/basic/basic-customer-client';
import { BasicStateClient } from '../../../services/basic/basic-state-client';
import { fetchAddressByCep } from '../../../services/cep-client';
import { UpdateCustomerCommand } from "../../../types/basic/customer/update-customer-command";

const customerClient = new BasicCustomerClient();
const stateClient = new BasicStateClient();

interface CustomerModalProps {
    visible: boolean;
    onClose: () => void;
    onSave: (data: UpdateCustomerCommand) => void;
    customer?: UpdateCustomerCommand;
    loading?: boolean;
}

const CustomerModal = ({
    visible,
    onClose,
    onSave,
    customer,
    loading
}: CustomerModalProps) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState<UpdateCustomerCommand | null>({
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
    const [states, setStates] = useState([]);
    const [loadingOptions, setLoadingOptions] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (visible) {
            loadOptions();
        }
    }, [visible]);

    useEffect(() => {
        if (customer) {
            setFormData({
                id: customer.id,
                name: customer.name || '',
                email: customer.email || '',
                phoneNumber: customer.phoneNumber || '',
                document: customer.document || '',
                address: customer.address
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
    }, [customer, visible]);

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
            setError(t('customers.requiredFields'));
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
                    {customer ? t('customers.edit') : t('customers.new')}
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
                                    label={t('customers.name')}
                                    id="name"
                                    value={formData.name}
                                    onChange={handleInputChange}
                                    required
                                />
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <FeniciaInput
                                    label={t('customers.document')}
                                    id="document"
                                    value={formData.document}
                                    onChange={handleInputChange}
                                    required
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <FeniciaInput
                            label={t('customers.email')}
                            id="email"
                            value={formData.email}
                            onChange={handleInputChange}
                            required
                        /> 

                    </div>

                    <div className="mb-3">
                        <FeniciaInput
                            label={t('customers.phone')}
                            id="phoneNumber"
                            value={formData.phoneNumber}
                            onChange={handleInputChange}
                        />
                    </div>

                    <h6 className="mt-4 mb-3">{t('customers.address')}</h6>

                    <CRow>
                        <CCol md={3}>
                            <div className="mb-3">
                                <FeniciaInput
                                    label={t('customers.zipCode')}
                                    id="zipCode"
                                    value={formData.address.zipCode}
                                    onChange={handleInputChange}
                                    onBlur={handleCepBlur}
                                    placeholder="00000-000"
                                    maxLength={9}
                                />
                            </div>
                        </CCol>
                        <CCol md={3}>
                            <div className="mb-3">
                                <FeniciaSelect
                                    id="stateId"
                                    data={states.map(state => ({ id: state.id, name: `${state.uf} - ${state.name}` }))}
                                    value={formData.address.stateId}
                                    onChange={handleInputChange}
                                    label={t('customers.state')}
                                />
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="city">{t('customers.city')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="city"
                                    name="city"
                                    value={formData.address.city}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaInput
                                    label={t('customers.street')}
                                    id="street"
                                    value={formData.address.street}
                                    onChange={handleInputChange}
                                />

                            </div>
                        </CCol>
                        <CCol md={2}>
                            <div className="mb-3">
                                <FeniciaInput
                                    label={t('customers.number')}
                                    id="number"
                                    value={formData.address.number}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <FeniciaInput
                                    label={t('customers.neighborhood')}
                                    id="neighborhood"
                                    value={formData.address.neighborhood}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <FeniciaInput
                            label={t('customers.complement')}
                            id="complement"
                            value={formData.address.complement}
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

export default CustomerModal;
