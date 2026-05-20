import { CCard, CCardBody, CCardHeader, CCol } from "@coreui/react";
import { Link } from "react-router-dom";
import { UserSubscriptionResponse } from "../../types/auth/user-subscription-response";

interface UserSubscriptionProps {
    subscriptions: UserSubscriptionResponse[];
}

const UserSubscriptionsSummary = ({ subscriptions = [] }: UserSubscriptionProps) => {
    return (
        <CCol md={4}>
            <CCard className="mb-4">
                <CCardHeader>
                    <strong>Resumo de Assinaturas</strong>
                </CCardHeader>
                <CCardBody>
                    <div className="text-center">
                        <h3 className="mb-2">{subscriptions?.length || 0}</h3>
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
    );
};

export default UserSubscriptionsSummary;
