import {
    CAlert,
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CContainer,
    CForm,
    CFormCheck,
    CRow,
    CSpinner
} from '@coreui/react';
import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthModuleClient from '../../services/auth/auth-module-client';
import AuthOrderClient from '../../services/auth/auth-order-client';

const moduleClient = new AuthModuleClient("http://localhost:5144");
const orderClient = new AuthOrderClient("http://localhost:5144");

const Subscription = () => {
    const navigate = useNavigate();
    const [modules, setModules] = useState([]);
    const [selectedModules, setSelectedModules] = useState([]);
    const [subscribedModuleIds, setSubscribedModuleIds] = useState([]);
    const [loading, setLoading] = useState(true);
    const [ordering, setOrdering] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);
    const [subscribedCount, setSubscribedCount] = useState(0);

    const newSelectedModules = useMemo(() =>
        selectedModules.filter(id => !subscribedModuleIds.includes(id))
        , [selectedModules, subscribedModuleIds]);

    const selectedCountNew = useMemo(() => newSelectedModules.length, [newSelectedModules]);

    const totalPrice = useMemo(() =>
        newSelectedModules.reduce((sum, id) => {
            const module = modules.find(m => m.id === id);
            return sum + (module?.price || 0);
        }, 0)
        , [newSelectedModules, modules]);

    useEffect(() => {
        loadModules();
    }, []);

    const loadModules = async () => {
        try {
            setLoading(true);
            setError(null);
            
            // Fetch available modules and subscribed modules in parallel
            const [modulesResponse, subscribedIds, profile] = await Promise.all([
                moduleClient.getModules(1, 50),
                moduleClient.getSubscribedModuleIds(),
                moduleClient.getProfile()
            ]);

            // Handle pagination response - response should have data array
            const modulesList = modulesResponse?.data || modulesResponse?.items || [];
            setModules(modulesList);
            setSubscribedModuleIds(subscribedIds);
            
            // Pre-select already subscribed modules (they will be disabled)
            setSelectedModules(subscribedIds);

            // Compute unique non-Basic subscribed modules count
            const nonBasicIds = profile?.subscriptions?.flatMap((subscription: any) =>
                subscription.modules.filter((m: any) => m.mType !== 'Basic').map((m: any) => m.id)
            ) || [];
            const uniqueNonBasicCount = new Set(nonBasicIds).size;
            setSubscribedCount(uniqueNonBasicCount);
        } catch (err) {
            console.error(err)
            console.error('Failed to load modules:', err);
            setError(err.response?.data?.title || 'Falha ao carregar módulos.');
        } finally {
            setLoading(false);
        }
    };

    const handleToggleModule = (moduleId) => {
        // Prevent toggling already subscribed modules
        if (subscribedModuleIds.includes(moduleId)) {
            return;
        }
        
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
            setSelectedModules([...new Set(modules.map(m => m.id))]);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Filter out already subscribed modules - only send new modules
        const newModules = selectedModules.filter(id => !subscribedModuleIds.includes(id));

        if (newModules.length === 0) {
            setError('Selecione pelo menos um módulo novo.');
            return;
        }

        setOrdering(true);
        setError(null);

        try {
            await orderClient.createOrder({
                modules: newModules
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

    const formatPrice = (price: number) => {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(price);
    };

    const SubscriptionSummary = ({ variant = 'default', isSticky = false }) => {
        return (
            <CCard className={`mb-3 shadow-lg ${isSticky ? 'sticky-bottom' : ''}`} style={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                color: 'white',
                border: 'none',
                top: isSticky ? '10px' : 'auto'
            }}>
                <CCardBody className="text-center py-4">
                    <CRow className="align-items-center">
                        <CCol md={6}>
                            <div className="h4 mb-1">{selectedCountNew} módulo(s) novo(s) selecionado(s)</div>
                            <div className="opacity-75">de {modules.length - subscribedModuleIds.length} disponíveis</div>
                        </CCol>
                        <CCol md={6} className="text-md-end">
                            <div className="h3 fw-bold mb-1">{formatPrice(totalPrice)}/mês</div>
                            <div className="small opacity-75">Total da assinatura</div>
                        </CCol>
                    </CRow>
                </CCardBody>
            </CCard>
        );
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

                                    <div className="alert alert-info mb-4 p-3">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <strong className="h6 mb-0">
                                                {subscribedCount} de {modules.length} módulo(s) ativo(s)
                                            </strong>
                                            <span className="badge bg-info">
                                                excluindo Básico
                                            </span>
                                        </div>
                                    </div>

                                    <div className="d-flex justify-content-between align-items-center mb-3">
                                        <span>
                                            {selectedCountNew} de {modules.length - subscribedModuleIds.length} novo(s) selecionado(s)
                                        </span>
                                        <CButton 
                                            color="outline-primary" 
                                            size="sm"
                                            onClick={handleSelectAll}
                                        >
                                            {selectedCountNew === (modules.length - subscribedModuleIds.length) ? 'Desmarcar novos' : 'Selecionar todos os novos'}
                                        </CButton>
                                    </div>

                                    <SubscriptionSummary variant="header" />

                                    <CForm onSubmit={handleSubmit}>
                                        <CRow className="g-4">
                                            {modules.map((module) => {
                                                const isSubscribed = subscribedModuleIds.includes(module.id);
                                                const isSelected = selectedModules.includes(module.id);
                                                
                                                return (
                                                    <CCol md={6} lg={4} key={module.id}>
                                                        <CCard
                                                            className={`h-100 shadow-sm ${
                                                                isSelected && !isSubscribed
                                                                ? 'border-primary bg-primary-subtle'
                                                                    : isSubscribed
                                                                    ? 'border-success bg-success-subtle'
                                                                    : 'border-light'
                                                            }`}
                                                            style={{
                                                                cursor: isSubscribed ? 'not-allowed' : 'pointer',
                                                                transition: 'all 0.2s',
                                                                opacity: isSubscribed ? 0.8 : 1
                                                            }}
                                                            onClick={() => handleToggleModule(module.id)}
                                                        >
                                                            <CCardBody>
                                                                <CFormCheck
                                                                    type="checkbox"
                                                                    id={`module-${module.id}`}
                                                                    label={
                                                                        <>
                                                                            <span className="text-dark fw-semibold">{module.name}</span>
                                                                            {isSubscribed && (
                                                                                <span className="ms-2 badge bg-success">
                                                                                    Já assinado
                                                                                </span>
                                                                            )}
                                                                        </>
                                                                    }
                                                                    checked={isSelected}
                                                                    onChange={() => handleToggleModule(module.id)}
                                                                    disabled={isSubscribed}
                                                                    className="mb-2"
                                                                />
                                                                <div className="fw-bold text-success h5 mb-1">
                                                                    {formatPrice(module.price || 0)}/mês
                                                                </div>
                                                            </CCardBody>
                                                        </CCard>
                                                    </CCol>
                                                );
                                            })}
                                        </CRow>

                                        <div className="mb-4">
                                            <div className="position-sticky bottom-0 z-3" style={{ top: 'auto' }}>
                                                <SubscriptionSummary variant="footer" isSticky={true} />
                                            </div>
                                        </div>

                                        <div className="mt-4 d-flex gap-2">
                                            <CButton 
                                                color="primary" 
                                                type="submit"
                                                disabled={ordering || selectedCountNew === 0}
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
