import React, { useEffect, useState } from 'react';
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
import BasicEmployeeClient from '../services/basic-employee-client';

const employeeClient = new BasicEmployeeClient("http://localhost:5002/");

const EmployeeModal = ({ 
    visible, 
    onClose, 
    onSave, 
    employee, 
    loading 
}) => {
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

    const handleSubmit = (e) => {
        e.preventDefault();
        setError(null);

        // Validation
        if (!formData.name || !formData.email || !formData.stateId || !formData.positionId) {
            setError('Nome, e-mail, estado e cargo são obrigatórios.');
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
                    {employee ? 'Editar Funcionário' : 'Novo Funcionário'}
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
                                <CFormLabel htmlFor="name">Nome *</CFormLabel>
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
                                <CFormLabel htmlFor="document">Documento (CPF/CNPJ)</CFormLabel>
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
                        <CFormLabel htmlFor="email">E-mail *</CFormLabel>
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
                                <CFormLabel htmlFor="phoneNumber">Telefone</CFormLabel>
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
                                <CFormLabel htmlFor="positionId">Cargo *</CFormLabel>
                                <CFormSelect
                                    id="positionId"
                                    name="positionId"
                                    value={formData.positionId}
                                    onChange={handleInputChange}
                                    disabled={loadingOptions}
                                    required
                                >
                                    <option value="">Selecione...</option>
                                    {positions.map(pos => (
                                        <option key={pos.id} value={pos.id}>
                                            {pos.name}
                                        </option>
                                    ))}
                                </CFormSelect>
                            </div>
                        </CCol>
                    </CRow>

                    <h6 className="mt-4 mb-3">Endereço</h6>

                    <CRow>
                        <CCol md={4}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="stateId">Estado *</CFormLabel>
                                <CFormSelect
                                    id="stateId"
                                    name="stateId"
                                    value={formData.stateId}
                                    onChange={handleInputChange}
                                    disabled={loadingOptions}
                                    required
                                >
                                    <option value="">Selecione...</option>
                                    {states.map(state => (
                                        <option key={state.id} value={state.id}>
                                            {state.name}
                                        </option>
                                    ))}
                                </CFormSelect>
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="city">Cidade</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="city"
                                    name="city"
                                    value={formData.city}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="zipCode">CEP</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="zipCode"
                                    name="zipCode"
                                    value={formData.zipCode}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="street">Rua</CFormLabel>
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
                                <CFormLabel htmlFor="number">Número</CFormLabel>
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
                                <CFormLabel htmlFor="neighborhood">Bairro</CFormLabel>
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
                        <CFormLabel htmlFor="complement">Complemento</CFormLabel>
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
                        Cancelar
                    </CButton>
                    <CButton 
                        color="primary" 
                        type="submit"
                        disabled={loading || loadingOptions}
                    >
                        {loading ? 'Salvando...' : 'Salvar'}
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default EmployeeModal;
