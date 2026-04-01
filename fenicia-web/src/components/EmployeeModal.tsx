import {
    CAlert,
    CButton,
    CCol,
    CForm,
    CFormInput,
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
import BasicEmployeeClient from '../services/basic/basic-employee-client';
import { fetchAddressByCep } from '../services/cep-client';

const employeeClient = new BasicEmployeeClient();

const EmployeeModal = ({ 
    visible, 
    onClose, 
    onSave, 
    employee, 
    loading 
}) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        phoneNumber: '',
        positionId: '',
        stateId: '',
        street: '',
        number: '',
        neighborhood: '',
        city: '',
        complement: '',
        zipCode: '',
        document: ''
    });
    const [states, setStates] = useState([]);
    const [positions, setPositions] = useState([]);
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
                name: employee.name || '',
                email: employee.email || '',
                phoneNumber: employee.phoneNumber || '',
                positionId: employee.positionId || '',
                stateId: employee.stateId || '',
                street: employee.street || '',
                number: employee.number || '',
                neighborhood: employee.neighborhood || '',
                city: employee.city || '',
                complement: employee.complement || '',
                zipCode: employee.zipCode || '',
                document: employee.document || ''
            });
        } else {
            setFormData({
                name: '',
                email: '',
                phoneNumber: '',
                positionId: '',
                stateId: '',
                street: '',
                number: '',
                neighborhood: '',
                city: '',
                complement: '',
                zipCode: '',
                document: ''
            });
        }
        setError(null);
    }, [employee, visible]);

    const loadOptions = async () => {
        try {
            setLoadingOptions(true);
            const [statesData, positionsData] = await Promise.all([
                employeeClient.getStates(),
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

        if (!formData.name || !formData.email || !formData.stateId || !formData.positionId) {
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
                                <CFormLabel htmlFor="name">{t('employees.name')} *</CFormLabel>
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
                                <CFormLabel htmlFor="document">{t('employees.document')}</CFormLabel>
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
                        <CFormLabel htmlFor="email">{t('employees.email')} *</CFormLabel>
                        <CFormInput
                            type="email"
                            id="email"
                            name="email"
                            value={formData.email}
                            onChange={handleInputChange}
                            required
                        />
                    </div>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="phoneNumber">{t('employees.phone')}</CFormLabel>
                                <CFormInput
                                    type="tel"
                                    id="phoneNumber"
                                    name="phoneNumber"
                                    value={formData.phoneNumber}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="positionId">{t('employees.position')} *</CFormLabel>
                                <CFormSelect
                                    id="positionId"
                                    name="positionId"
                                    value={formData.positionId}
                                    onChange={handleInputChange}
                                    disabled={loadingOptions}
                                    required
                                >
                                    <option value="">{t('common.select')}...</option>
                                    {positions.map(pos => (
                                        <option key={pos.id} value={pos.id}>
                                            {pos.name}
                                        </option>
                                    ))}
                                </CFormSelect>
                            </div>
                        </CCol>
                    </CRow>

                    <h6 className="mt-4 mb-3">{t('employees.address')}</h6>

                    <CRow>
                        <CCol md={3}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="zipCode">{t('employees.zipCode')}</CFormLabel>
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
                                <CFormLabel htmlFor="stateId">{t('employees.state')} *</CFormLabel>
                                <CFormSelect
                                    id="stateId"
                                    name="stateId"
                                    value={formData.stateId}
                                    onChange={handleInputChange}
                                    disabled={loadingOptions}
                                    required
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
                                <CFormLabel htmlFor="city">{t('employees.city')}</CFormLabel>
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
                                <CFormLabel htmlFor="street">{t('employees.street')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="street"
                                    name="street"
                                    value={formData.street}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={3}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="number">{t('employees.number')}</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="number"
                                    name="number"
                                    value={formData.number}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={3}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="neighborhood">{t('employees.neighborhood')}</CFormLabel>
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
                        <CFormLabel htmlFor="complement">{t('employees.complement')}</CFormLabel>
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

export default EmployeeModal;
