import {
    CAlert,
    CButton,
    CCol,
    CForm,
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
import BasicEmployeeClient from '../../../services/basic/basic-employee-client';
import { BasicStateClient } from '../../../services/basic/basic-state-client';
import { fetchAddressByCep } from '../../../services/cep-client';
import { UpdateEmployeeCommand } from "../../../types/basic/employee/update-employee-command";
import { DataSourceItem } from "../../../types/basic/product-category/add-product-category-command";
import { GetAllStateResponse } from "../../../types/basic/state/get-all-state-response";

const employeeClient = new BasicEmployeeClient();
const stateClient = new BasicStateClient();

const EmployeeModal = ({ 
    visible, 
    onClose, 
    onSave, 
    employee, 
    loading 
}) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState<UpdateEmployeeCommand>({
        name: '',
        email: '',
        phoneNumber: '',
        positionId: '',
        id: '',
        document: '',
        address: {
            street: '',
            number: '',
            neighborhood: '',
            city: '',
            complement: '',
            zipCode: '',
            stateId: '',
            country: ''
        }
    });
    const [states, setStates] = useState<GetAllStateResponse[]>([]);
    const [positions, setPositions] = useState<DataSourceItem[]>([]);
    const [loadingOptions, setLoadingOptions] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (visible) {
            loadOptions();
        }
    }, [visible]);

    useEffect(() => {
        if (employee) {
            setFormData({
                    id: employee.id,
                    name: employee.name || '',
                    email: employee.email || '',
                    phoneNumber: employee.phoneNumber || '',
                    positionId: employee.positionId || '',
                    address: employee.address ? {
                        street: employee.address.street || '',
                        number: employee.address.number || '',
                        neighborhood: employee.address.neighborhood || '',
                        city: employee.address.city || '',
                        complement: employee.address.complement || '',
                        zipCode: employee.address.zipCode || '',
                        stateId: employee.address.stateId || '',
                        country: employee.address.country || ''
                    } : {
                        street: '',
                        number: '',
                        neighborhood: '',
                        city: '',
                        complement: '',
                        zipCode: '',
                        stateId: '',
                        country: ''
                    },
                    document: employee.document || ''
                });
        } else {
            setFormData({
                id: null,
                name: '',
                email: '',
                phoneNumber: '',
                positionId: '',
                address: {
                    street: '',
                    number: '',
                    neighborhood: '',
                    city: '',
                    complement: '',
                    zipCode: '',
                    stateId: '',
                    country: ''
                },
                document: ''
            });
        }
        setError(null);
    }, [employee, visible]);

    const loadOptions = async () => {
        try {
            setLoadingOptions(true);
            const [statesData, positionsData] = await Promise.all([
                stateClient.getStates(),
                employeeClient.getPositions()
            ]);
            setStates(statesData || []);
            setPositions(positionsData || []);
        } catch (err) {
            console.error('Failed to load options:', err);
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

        console.log(formData)

        if (!formData.name || !formData.email || !formData.address?.stateId || !formData.positionId) {
            setError(t('employees.requiredFields'));
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
                    {employee ? t('employees.edit') : t('employees.new')}
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
                                    label="employees.name"
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
                                    label="employees.document"
                                    id="document"
                                    value={formData.document}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <FeniciaInput 
                            label="employees.email"
                            id="email"
                            type="email"
                            value={formData.email}
                            onChange={handleInputChange}
                            required
                        />
                    </div>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaInput 
                                    label="employees.phone"
                                    id="phoneNumber"
                                    type="tel"
                                    value={formData.phoneNumber}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaSelect
                                    label="employees.position"
                                    id="positionId"
                                    data={positions}
                                    value={formData.positionId}
                                    onChange={handleInputChange}
                                    loadingOptions={loadingOptions}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <h6 className="mt-4 mb-3">{t('employees.address')}</h6>

                    <CRow>
                        <CCol md={3}>
                            <div className="mb-3">
                                <FeniciaInput 
                                    label="employees.zipCode"
                                    id="zipCode"
                                    value={formData.address?.zipCode}
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
                                    label="employees.state"
                                    id="stateId"
                                    data={states}
                                    value={formData.address?.stateId}
                                    onChange={handleInputChange}
                                    loadingOptions={loadingOptions}
                                />

                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <FeniciaInput 
                                    label="employees.city"
                                    id="city"
                                    value={formData.address?.city}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">

                                <FeniciaInput
                                    label="employees.street"
                                    id="street"
                                    value={formData.address?.street}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={3}>
                            <div className="mb-3">
                                <FeniciaInput 
                                    label="employees.number"
                                    id="number"
                                    value={formData.address?.number}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={3}>
                            <div className="mb-3">
                                <FeniciaInput
                                    label="employees.neighborhood"
                                    id="neighborhood"
                                    value={formData.address?.neighborhood}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <FeniciaInput
                            label="employees.complement"
                            id="complement"
                            value={formData.address?.complement}
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

export default EmployeeModal;
