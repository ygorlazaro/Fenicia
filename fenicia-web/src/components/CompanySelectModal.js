import React from 'react';
import {
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CModal,
    CModalBody,
    CModalHeader,
    CModalTitle,
    CSpinner
} from '@coreui/react';

const CompanySelectModal = ({ 
    visible, 
    companies, 
    loading, 
    error,
    onSelect 
}) => {
    console.log('CompanySelectModal render:', { visible, companies, loading, error, count: companies?.length });
    
    return (
        <CModal 
            visible={visible} 
            backdrop="static" 
            size="lg"
            onClose={() => {}} // Prevent closing
        >
            <CModalHeader>
                <CModalTitle>Selecionar Empresa</CModalTitle>
            </CModalHeader>
            <CModalBody>
                {loading && (
                    <div className="text-center py-4">
                        <CSpinner color="primary" />
                        <p className="mt-2">Carregando empresas...</p>
                    </div>
                )}

                {error && (
                    <div className="alert alert-danger" role="alert">
                        {error}
                    </div>
                )}

                {!loading && !error && companies.length === 0 && (
                    <div className="alert alert-warning" role="alert">
                        Nenhuma empresa encontrada para este usuário.
                    </div>
                )}

                <div className="list-group">
                    {companies.map((company) => (
                        <button
                            key={company.id}
                            type="button"
                            className="list-group-item list-group-item-action"
                            onClick={() => onSelect(company)}
                        >
                            <div className="d-flex w-100 justify-content-between">
                                <h6 className="mb-1">{company.name}</h6>
                            </div>
                            <div className="d-flex gap-2">
                                <small className="text-muted">CNPJ: {company.cnpj}</small>
                                {company.isDefault && (
                                    <span className="badge bg-primary">Padrão</span>
                                )}
                            </div>
                        </button>
                    ))}
                </div>
            </CModalBody>
        </CModal>
    );
};

export default CompanySelectModal;
