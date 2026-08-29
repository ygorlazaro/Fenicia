// @ts-nocheck
import { cilCalendar } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { Link } from "react-router-dom";
import { UserSubscriptionResponse } from "../../types/auth/user-subscription-response";

interface UserSubscriptionProps {
    subscriptions: UserSubscriptionResponse[];
}

const getStatusColor = (status: string) => {
    const colors = {
        Active: "success",
        Inactive: "secondary",
        Pending: "warning",
        Cancelled: "danger"
    };
    return colors[status] || "secondary";
};

const formatDate = (dateString: string | number | Date) => {
    if (!dateString) {
        return "-";
    }
    return new Date(dateString).toLocaleDateString("pt-BR");
};

const UserSubscriptionsDetail = ({ subscriptions = [] }: UserSubscriptionProps) => {
    return (
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
                        {subscriptions.length > 0 ? (
                            subscriptions.map((subscription, idx: number) => (
                                <div key={subscription.id} className={idx > 0 ? "mt-4" : ""}>
                                    {idx > 0 && <hr />}
                                    <div className="d-flex justify-content-between align-items-center mb-3">
                                        <div>
                                            <h5 className="mb-1">{subscription.companyName}</h5>
                                            <small className="text-muted">
                                                Início: {formatDate(subscription.startDate)}
                                                {subscription.endDate && ` | Fim: ${formatDate(subscription.endDate)}`}
                                            </small>
                                        </div>
                                        <CBadge color={getStatusColor(subscription.status)}>{subscription.status === "Active" ? "Ativo" : subscription.status}</CBadge>
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
                                                        <CTableDataCell>{formatDate(subscription.startDate)}</CTableDataCell>
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
    );
};

export default UserSubscriptionsDetail;
