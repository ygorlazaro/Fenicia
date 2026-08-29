import { CCard, CCardBody, CCol, CRow } from "@coreui/react";
import formatCurrency from "../../utils/format-currency";

interface SubscriptionSummaryProps {
    isSticky?: boolean;
    selectedCountNew: number;
    modulesCount: number;
    subscribedModulesCount: number;
    totalPrice: number;
}

const SubscriptionSummary = ({ isSticky = false, selectedCountNew, modulesCount, subscribedModulesCount, totalPrice }: SubscriptionSummaryProps) => {
    return (
        <CCard
            className={`mb-3 shadow-lg ${isSticky ? "sticky-bottom" : ""}`}
            style={{
                background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
                color: "white",
                border: "none",
                top: isSticky ? "10px" : "auto"
            }}
        >
            <CCardBody className="text-center py-4">
                <CRow className="align-items-center">
                    <CCol md={6}>
                        <div className="h4 mb-1">{selectedCountNew} módulo(s) novo(s) selecionado(s)</div>
                        <div className="opacity-75">de {modulesCount - subscribedModulesCount} disponíveis</div>
                    </CCol>
                    <CCol md={6} className="text-md-end">
                        <div className="h3 fw-bold mb-1">{formatCurrency(totalPrice)}/mês</div>
                        <div className="small opacity-75">Total da assinatura</div>
                    </CCol>
                </CRow>
            </CCardBody>
        </CCard>
    );
};

export default SubscriptionSummary;
