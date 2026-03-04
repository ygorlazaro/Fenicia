import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  CCard,
  CCardBody,
  CCardHeader,
  CContainer,
  CRow,
  CCol,
  CSpinner,
  CAlert,
  CBadge,
  CTable,
  CTableBody,
  CTableDataCell,
  CTableHead,
  CTableHeaderCell,
  CTableRow,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilBuilding, cilCalendar, cilCheck, cilUser } from '@coreui/icons';
import AuthProfileClient from '../../services/auth-profile-client';

const profileClient = new AuthProfileClient();

const Profile = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [profile, setProfile] = useState(null);

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await profileClient.getProfile();
      setProfile(data);
    } catch (err) {
      console.error('Failed to load profile:', err);
      setError(err.response?.data?.title || 'Falha ao carregar perfil.');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const getStatusColor = (status) => {
    const colors = {
      Active: 'success',
      Inactive: 'secondary',
      Pending: 'warning',
      Cancelled: 'danger',
    };
    return colors[status] || 'secondary';
  };

  if (loading) {
    return (
      <CContainer className="py-4">
        <div className="text-center py-5">
          <CSpinner color="primary" size="lg" />
          <p className="mt-3">Carregando perfil...</p>
        </div>
      </CContainer>
    );
  }

  if (error) {
    return (
      <CContainer className="py-4">
        <CAlert color="danger">{error}</CAlert>
      </CContainer>
    );
  }

  if (!profile) {
    return (
      <CContainer className="py-4">
        <CAlert color="warning">Perfil não encontrado.</CAlert>
      </CContainer>
    );
  }

  return (
    <CContainer className="py-4">
      <CRow>
        <CCol md={8}>
          {/* User Information */}
          <CCard className="mb-4">
            <CCardHeader>
              <strong>
                <CIcon icon={cilUser} className="me-2" />
                Informações do Usuário
              </strong>
            </CCardHeader>
            <CCardBody>
              <CRow>
                <CCol md={6}>
                  <h6 className="text-muted">Nome</h6>
                  <p className="fs-5">{profile.name}</p>
                </CCol>
                <CCol md={6}>
                  <h6 className="text-muted">E-mail</h6>
                  <p className="fs-5">{profile.email}</p>
                </CCol>
              </CRow>
            </CCardBody>
          </CCard>

          {/* Companies */}
          <CCard className="mb-4">
            <CCardHeader>
              <strong>
                <CIcon icon={cilBuilding} className="me-2" />
                Empresas ({profile.companies?.length || 0})
              </strong>
            </CCardHeader>
            <CCardBody>
              {profile.companies && profile.companies.length > 0 ? (
                <CTable hover responsive>
                  <CTableHead>
                    <CTableRow>
                      <CTableHeaderCell>Nome</CTableHeaderCell>
                      <CTableHeaderCell>CNPJ</CTableHeaderCell>
                      <CTableHeaderCell className="text-center">Padrão</CTableHeaderCell>
                    </CTableRow>
                  </CTableHead>
                  <CTableBody>
                    {profile.companies.map((company) => (
                      <CTableRow key={company.id}>
                        <CTableDataCell>{company.name}</CTableDataCell>
                        <CTableDataCell>{company.cnpj}</CTableDataCell>
                        <CTableDataCell className="text-center">
                          {company.isDefault ? (
                            <CBadge color="success">
                              <CIcon icon={cilCheck} /> Sim
                            </CBadge>
                          ) : (
                            <CBadge color="secondary">Não</CBadge>
                          )}
                        </CTableDataCell>
                      </CTableRow>
                    ))}
                  </CTableBody>
                </CTable>
              ) : (
                <p className="text-muted">Nenhuma empresa encontrada.</p>
              )}
            </CCardBody>
          </CCard>
        </CCol>

        <CCol md={4}>
          {/* Subscriptions Summary */}
          <CCard className="mb-4">
            <CCardHeader>
              <strong>Resumo de Assinaturas</strong>
            </CCardHeader>
            <CCardBody>
              <div className="text-center">
                <h3 className="mb-2">{profile.subscriptions?.length || 0}</h3>
                <p className="text-muted">Assinatura(ões) Ativa(s)</p>
                <hr />
                <div className="d-grid gap-2">
                  <Link to="/subscription" className="btn btn-primary">
                    Assinar Novos Módulos
                  </Link>
                </div>
              </div>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>

      {/* Subscriptions Details */}
      <CRow>
        <CCol md={12}>
          <CCard>
            <CCardHeader>
              <strong>
                <CIcon icon={cilCalendar} className="me-2" />
                Assinaturas e Módulos
              </strong>
            </CCardHeader>
            <CCardBody>
              {profile.subscriptions && profile.subscriptions.length > 0 ? (
                profile.subscriptions.map((subscription, idx) => (
                  <div key={subscription.id} className={idx > 0 ? 'mt-4' : ''}>
                    {idx > 0 && <hr />}
                    <div className="d-flex justify-content-between align-items-center mb-3">
                      <div>
                        <h5 className="mb-1">{subscription.companyName}</h5>
                        <small className="text-muted">
                          Início: {formatDate(subscription.startDate)}
                          {subscription.endDate && ` | Fim: ${formatDate(subscription.endDate)}`}
                        </small>
                      </div>
                      <CBadge color={getStatusColor(subscription.status)} size="lg">
                        {subscription.status === 'Active' ? 'Ativo' : subscription.status}
                      </CBadge>
                    </div>

                    {subscription.modules && subscription.modules.length > 0 ? (
                      <CTable hover responsive borderless>
                        <CTableHead>
                          <CTableRow>
                            <CTableHeaderCell>Módulo</CTableHeaderCell>
                            <CTableHeaderCell>Tipo</CTableHeaderCell>
                            <CTableHeaderCell>Assinado em</CTableHeaderCell>
                          </CTableRow>
                        </CTableHead>
                        <CTableBody>
                          {subscription.modules.map((module) => (
                            <CTableRow key={module.id}>
                              <CTableDataCell>
                                <strong>{module.name}</strong>
                              </CTableDataCell>
                              <CTableDataCell>
                                <CBadge color="info">{module.type}</CBadge>
                              </CTableDataCell>
                              <CTableDataCell>{formatDate(module.subscribedAt)}</CTableDataCell>
                            </CTableRow>
                          ))}
                        </CTableBody>
                      </CTable>
                    ) : (
                      <p className="text-muted">Nenhum módulo assinado nesta empresa.</p>
                    )}
                  </div>
                ))
              ) : (
                <div className="text-center py-4">
                  <CIcon icon={cilCalendar} size="3xl" className="text-muted mb-3" />
                  <p className="text-muted">Você não possui assinaturas ativas.</p>
                  <Link to="/subscription" className="btn btn-primary">
                    Assinar Módulos Agora
                  </Link>
                </div>
              )}
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
    </CContainer>
  );
};

export default Profile;
