import {
    CAlert,
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CForm
} from "@coreui/react";
import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthLayout } from "../../../components";
import { FeniciaInput } from "../../../components/fenicia/fenicia-input";
import AuthRegisterClient from '../../../services/auth/auth-register-client';
import { CreateNewUserCommand } from "../../../types/auth/create-new-user-command";

const registerClient = new AuthRegisterClient();

const AuthRegister = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState<CreateNewUserCommand>({
        name: '',
        email: '',
        password: '',
        company: {
            name: '',
            cnpj: ''
        }
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            await registerClient.register(formData);

            navigate('/auth/login', { 
                state: { message: 'Conta criada com sucesso! Faça login para continuar.' }
            });
        } catch (err) {
            console.error('Registration failed:', err);
            setError(err.response?.data?.title || 'Falha ao criar conta. Tente novamente.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthLayout>
            <CCard className="mb-4 shadow-sm">
                <CCardHeader className="bg-primary text-white">
                    <strong>Criar Conta</strong>
                </CCardHeader>
                <CCardBody>
                    {error && (
                        <CAlert color="danger" dismissible onClose={() => setError(null)}>
                            {error}
                        </CAlert>
                    )}
                    <CForm onSubmit={handleSubmit}>
                        <div className="mb-3">
                            <FeniciaInput
                                type="text"
                                id="inputNameFenicia"
                                label="Nome"
                                value={formData.name}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <FeniciaInput
                                type="email"
                                id="inputEmailFenicia"
                                label="E-mail"
                                value={formData.email}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <FeniciaInput
                                type="password"
                                id="inputPasswordFenicia"
                                label="Senha"
                                value={formData.password}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <hr className="my-4" />
                        <h6 className="mb-3">Dados da Empresa</h6>
                        <div className="mb-3">
                            <FeniciaInput
                                type="text"
                                id="inputCompanyNameFenicia"
                                label="Nome da Empresa"
                                value={formData.company.name}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <FeniciaInput
                                type="text"
                                id="inputCompanyCnpjFenicia"
                                label="CNPJ"
                                value={formData.company.cnpj}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="d-grid gap-2">
                            <CButton
                                color="primary"
                                type="submit"
                                disabled={loading}
                            >
                                {loading ? 'Criando conta...' : 'Criar conta'}
                            </CButton>
                        </div>

                        <div className="text-center mt-3">
                            <Link to="/auth/login" className="text-decoration-none">
                                Já tem uma conta? Entrar
                            </Link>
                        </div>
                    </CForm>
                </CCardBody>
            </CCard>
        </AuthLayout>
    )
};

export default AuthRegister;
