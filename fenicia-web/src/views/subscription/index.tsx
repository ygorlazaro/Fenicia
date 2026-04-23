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
import AuthSubscriptionClient from '../../services/auth/auth-subscription-client';
import { GetModuleResponse } from "../../types/auth/get-module-response";
import formatCurrency from '../../utils/format-currency';
import SubscriptionHeader from './subscription-header';
import SubscriptionSummary from './subscription-summary';

const moduleClient = new AuthModuleClient("http://localhost:5144");
const orderClient = new AuthOrderClient("http://localhost:5144");
const profileClient = new AuthSubscriptionClient("http://localhost:5144");

const Subscription = () => {
    const navigate = useNavigate();
    const [modules, setModules] = useState<GetModuleResponse[]>([]);
    const [selectedModules, setSelectedModules] = useState<string[]>([]);
    const [subscribedModuleIds, setSubscribedModuleIds] = useState<string[]>([]);
    const [loading, setLoading] = useState(true);
    const [ordering, setOrdering] = useState(false);
    const [error, setError] = useState<string | null>(null);
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

            const [modulesResponse, subscribedIds, profile] = await Promise.all([
                moduleClient.getModules(1, 50),
                moduleClient.getSubscribedModuleIds(),
                profileClient.getProfile()
            ]);

            setModules(modulesResponse.data);
            setSubscribedModuleIds(subscribedIds);

            setSelectedModules(subscribedIds);

            const nonBasicIds = profile?.subscriptions?.flatMap((subscription) =>
                subscription.modules.filter((m) => m.type !== "Basic").map((m) => m.id)
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

    const handleToggleModule = (moduleId: string) => {
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

    const handleSubmit = async (e: { preventDefault: () => void; }) => {
        e.preventDefault();

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
                                    <SubscriptionHeader subscribedCount={subscribedCount} selectedCountNew={selectedCountNew} handleSelectAll={handleSelectAll} modulesCount={modules.length} subscribedModulesCount={subscribedCount}  />

                                    <SubscriptionSummary selectedCountNew={selectedCountNew} modulesCount={modules.length} subscribedModulesCount={subscribedModuleIds.length} totalPrice={totalPrice} />

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
                                                                            <span className="fw-semibold">{module.name}</span>
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
                                                                    {formatCurrency(module.price || 0)}/mês
                                                                </div>
                                                            </CCardBody>
                                                        </CCard>
                                                    </CCol>
                                                );
                                            })}
                                        </CRow>

                                        <div className="mb-4">
                                            <div className="position-sticky bottom-0 z-3" style={{ top: 'auto' }}>
                                                <SubscriptionSummary selectedCountNew={selectedCountNew} modulesCount={modules.length} subscribedModulesCount={subscribedModuleIds.length} totalPrice={totalPrice} />
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
