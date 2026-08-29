// @ts-nocheck
import { cilBuilding } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { UserCompanyResponse } from "../../types/auth/user-company-response";

interface UserProfileProps {
    companies: UserCompanyResponse[];
}

const UserCompanies = ({ companies = [] }: UserProfileProps) => {
    return (
        <CCard className="mb-4">
            <CCardHeader>
                <strong>
                    <CIcon icon={cilBuilding} className="me-2" />
                    Empresas ({companies.length || 0})
                </strong>
            </CCardHeader>
            <CCardBody>
                {companies.length > 0 ? (
                    <CTable hover responsive>
                        <CTableHead>
                            <CTableRow>
                                <CTableHeaderCell>Nome</CTableHeaderCell>
                                <CTableHeaderCell>CNPJ</CTableHeaderCell>
                            </CTableRow>
                        </CTableHead>
                        <CTableBody>
                            {companies.map((company) => (
                                <CTableRow key={company.id}>
                                    <CTableDataCell>{company.name}</CTableDataCell>
                                    <CTableDataCell>{company.cnpj}</CTableDataCell>
                                </CTableRow>
                            ))}
                        </CTableBody>
                    </CTable>
                ) : (
                    <p className="text-muted">Nenhuma empresa encontrada.</p>
                )}
            </CCardBody>
        </CCard>
    );
};

export default UserCompanies;
