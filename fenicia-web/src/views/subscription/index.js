import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CContainer,
    CFormCheck,
    CRow,
    CAlert,
    CSpinner, CForm
} from '@coreui/react';
import AuthModuleClient from "src/services/auth-module-client";
import AuthOrderClient from "src/services/auth-order-client";

const moduleClient = new AuthModuleClient("http://localhost:5144");
const orderClient = new AuthOrderClient("http://localhost:5144");

const Subscription = () => {
    const navigate = useNavigate();
    const [modules, setModules] = useState([]);
    const [selectedModules, setSelectedModules] = useState([]);
    const [loading, setLoading] = useState(true);
    const [ordering, setOrdering] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);

    useEffect(() => {
        loadModules();
    }, []);

    const loadModules = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await moduleClient.getModules(1, 50);
            console.log('Modules response:', response);
            
            // Handle pagination response - response should have data array
            const modulesList = response?.data || response?.items || [];
            setModules(modulesList);
        } catch (err) {
            console.error('Failed to load modules:', err);
            setError(err.response?.data?.title || 'Falha ao carregar módulos.');
        } finally {
            setLoading(false);
        }
    };

    const handleToggleModule = (moduleId) => {
        setSelectedModules(prev => 
            prev.includes(moduleId)
                ? prev.filter(id => id !== moduleId)
                : [...prev, moduleId]
        );
    };

    const handleSelectAll = () => {
        if (selectedModules.length === modules.length) {
            setSelectedModules([]);
        } else {
            setSelectedModules(modules.map(m => m.id));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (selectedModules.length === 0) {
            setError('Selecione pelo menos um módulo.');
            return;
        }

        setOrdering(true);
        setError(null);

        try {
            await orderClient.createOrder({
                modules: selectedModules
            });
            setSuccess(true);
            
            // Redirect to dashboard after 3 seconds
            setTimeout(() => {
                navigate('/dashboard');
            }, 3000);
        } catch (err) {
            console.error('Failed to create order:', err);
            setError(err.response?.data?.title || 'Falha ao criar assinatura. Tente novamente.');
        } finally {
            setOrdering(false);
        }
    };

    const formatPrice = (price) => {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(price);
    };

    return (
        <CContainer className="py-4">
            <CRow className="justify-content-center">
                <CCol lg={10}>
                    <CCard className="mb-4">
                        <CCardHeader>
                            <strong>Assinar Módulos</strong>
                        </CCardHeader>
                        <CCardBody>
                            {error && (
                                <CAlert color="danger" dismissible>
                                    {error}
                                </CAlert>
                            )}

                            {success && (
                                <CAlert color="success" dismissible>
                                    <strong>Assinatura criada com sucesso!</strong> Você será redirecionado para o dashboard.
                                </CAlert>
                            )}

                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">Carregando módulos...</p>
                                </div>
                            )}

                            {!loading && !success && (
                                <>
                                    <p className="text-muted mb-4">
                                        Selecione os módulos que deseja assinar para sua empresa.
                                    </p>

                                    <div className="d-flex justify-content-between align-items-center mb-3">
                                        <span>
                                            {selectedModules.length} de {modules.length} módulo(s) selecionado(s)
                                        </span>
                                        <CButton 
                                            color="outline-primary" 
                                            size="sm"
                                            onClick={handleSelectAll}
                                        >
                                            {selectedModules.length === modules.length ? 'Desmarcar todos' : 'Selecionar todos'}
                                        </CButton>
                                    </div>

                                    <CForm onSubmit={handleSubmit}>
                                        <CRow className="g-4">
                                            {modules.map((module) => (
                                                <CCol md={6} lg={4} key={module.id}>
                                                    <CCard 
                                                        className={`h-100 ${
                                                            selectedModules.includes(module.id) 
                                                                ? 'border-primary' 
                                                                : ''
                                                        }`}
                                                        style={{
                                                            cursor: 'pointer',
                                                            transition: 'all 0.2s'
                                                        }}
                                                        onClick={() => handleToggleModule(module.id)}
                                                    >
                                                        <CCardBody>
                                                            <CFormCheck
                                                                type="checkbox"
                                                                id={`module-${module.id}`}
                                                                label={module.name}
                                                                checked={selectedModules.includes(module.id)}
                                                                onChange={() => handleToggleModule(module.id)}
                                                                className="mb-2"
                                                            />
                                                            <div className="text-muted small mb-2">
                                                                Tipo: {module.type}
                                                            </div>
                                                            <div className="fw-bold text-primary">
                                                                {module.price ? formatPrice(module.price) : 'Sob consulta'}
                                                            </div>
                                                        </CCardBody>
                                                    </CCard>
                                                </CCol>
                                            ))}
                                        </CRow>

                                        <div className="mt-4 d-flex gap-2">
                                            <CButton 
                                                color="primary" 
                                                type="submit"
                                                disabled={ordering || selectedModules.length === 0}
                                            >
                                                {ordering ? 'Processando...' : 'Criar Assinatura'}
                                            </CButton>
                                            <CButton 
                                                color="secondary"
                                                type="button"
                                                onClick={() => navigate('/dashboard')}
                                            >
                                                Cancelar
                                            </CButton>
                                        </div>
                                    </CForm>
                                </>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        </CContainer>
    );
};

export default Subscription;
