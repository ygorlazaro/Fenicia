import { CCol, CContainer, CRow } from "@coreui/react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../../store";
import { setCompany } from "../../../features/auth/authSlice";
import AuthCompanyClient from "../../../services/auth/auth-company-client";
import { GetCompaniesByUserResponse } from "../../../types/auth/get-companies-by-user-response";
import CompanySelectModal from "./company-select-modal";

const companyClient = new AuthCompanyClient();

const CompanySelect = () => {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const [companies, setCompanies] = useState<GetCompaniesByUserResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selected, setSelected] = useState(false);

    useEffect(() => {
        loadCompanies();
    }, []);

    const loadCompanies = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await companyClient.getCompaniesByUser(1, 50);

            setCompanies(response.data);

            if (!response.data || response.data.length === 0) {
                setError("Nenhuma empresa encontrada para este usuário.");
            }
        } catch (err) {
            setError(err.response?.data?.title || err.message || "Falha ao carregar empresas.");
        } finally {
            setLoading(false);
        }
    };

    const handleSelectCompany = (company: GetCompaniesByUserResponse) => {
        if (selected) return;
        setSelected(true);

        companyClient.setCompanyId(company.id);
        dispatch(setCompany({ companyId: company.id, companyName: company.name }));

        navigate("/dashboard");
    };

    return (
        <CContainer className="py-4">
            <CRow className="justify-content-center">
                <CCol md={8}>
                    <CompanySelectModal visible={true} companies={companies} loading={loading} error={error} onSelect={handleSelectCompany} />
                </CCol>
            </CRow>
        </CContainer>
    );
};

export default CompanySelect;
