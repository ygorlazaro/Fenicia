import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { CContainer, CRow, CCol } from '@coreui/react';
import CompanySelectModal from '../../../components/CompanySelectModal';
import AuthCompanyClient from '../../../services/auth-company-client';
import { setCompanyId } from '../../../services/client';

const companyClient = new AuthCompanyClient("http://localhost:5144");

const CompanySelect = () => {
    const navigate = useNavigate();
    const [companies, setCompanies] = useState([]);
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
            console.log('Fetching companies...');
            const response = await companyClient.getCompaniesByUser(1, 50);
            console.log('Companies response:', response);

            // Handle both array and paginated response
            const companiesList = Array.isArray(response) ? response : response.items || response.data || [];
            console.log('Companies list:', companiesList);
            setCompanies(companiesList);
            
            if (companiesList.length === 0) {
                setError('Nenhuma empresa encontrada para este usuário.');
            }
        } catch (err) {
            console.error('Failed to load companies:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || err.message || 'Falha ao carregar empresas.');
        } finally {
            setLoading(false);
        }
    };

    const handleSelectCompany = (company) => {
        if (selected) return; // Prevent multiple selections
        setSelected(true);
        
        // Persist company ID and name to localStorage
        setCompanyId(company.id);
        localStorage.setItem('company_name', company.name);
        
        // Redirect to dashboard
        navigate('/dashboard');
    };

    return (
        <CContainer className="py-4">
            <CRow className="justify-content-center">
                <CCol md={8}>
                    <CompanySelectModal
                        visible={true}
                        companies={companies}
                        loading={loading}
                        error={error}
                        onSelect={handleSelectCompany}
                    />
                </CCol>
            </CRow>
        </CContainer>
    );
};

export default CompanySelect;
