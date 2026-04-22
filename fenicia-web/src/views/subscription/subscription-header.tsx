import { CButton } from "@coreui/react";

interface SubscriptionHeaderProps {
    subscribedCount: number;
    selectedCountNew: number;
    modulesCount: number;
    subscribedModulesCount: number;
    handleSelectAll: () => void;
}


export function SubscriptionHeader({
    subscribedCount,
    selectedCountNew,
    modulesCount,
    subscribedModulesCount,
    handleSelectAll
}: SubscriptionHeaderProps) {
    return <><p className="text-muted mb-4">
        Selecione os módulos que deseja assinar para sua empresa.
    </p>

        <div className="alert alert-info mb-4 p-3">
            <div className="d-flex justify-content-between align-items-center">
                <strong className="h6 mb-0">
                    {subscribedCount} de {modulesCount} módulo(s) ativo(s)
                </strong>
                <span className="badge bg-info">
                    excluindo Básico
                </span>
            </div>
        </div>

        <div className="d-flex justify-content-between align-items-center mb-3">
            <span>
                {selectedCountNew} de {modulesCount - subscribedModulesCount} novo(s) selecionado(s)
            </span>
            <CButton color="outline-primary" size="sm" onClick={handleSelectAll}>
                {selectedCountNew === modulesCount - subscribedModulesCount ? 'Desmarcar novos' : 'Selecionar todos os novos'}
            </CButton>
        </div></>;
}
