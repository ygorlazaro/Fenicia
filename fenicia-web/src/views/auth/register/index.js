import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import {
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CForm,
    CFormInput,
    CFormLabel,
    CAlert
} from "@coreui/react";
import AuthLayout from 'src/components/AuthLayout';
import AuthRegisterClient from '../../../services/auth-register-client';

const registerClient = new AuthRegisterClient("http://localhost:5144");

const AuthRegister = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        password: '',
        companyName: '',
        cnpj: ''
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
            await registerClient.register({
                name: formData.name,
                email: formData.email,
                password: formData.password,
                company: {
                    name: formData.companyName,
                    cnpj: formData.cnpj,
                    timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone
                }
            });

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
                            <CFormLabel htmlFor="inputName">Nome</CFormLabel>
                            <CFormInput
                                type="text"
                                id="inputName"
                                name="name"
                                placeholder="Seu nome completo"
                                value={formData.name}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <CFormLabel htmlFor="inputEmail">E-mail</CFormLabel>
                            <CFormInput
                                type="email"
                                id="inputEmail"
                                name="email"
                                placeholder="name@example.com"
                                value={formData.email}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <CFormLabel htmlFor="inputPassword">Senha</CFormLabel>
                            <CFormInput
                                type="password"
                                id="inputPassword"
                                name="password"
                                placeholder="senha"
                                value={formData.password}
                                onChange={handleInputChange}
                                required
                                minLength={6}
                            />
                        </div>
                        <hr className="my-4" />
                        <h6 className="mb-3">Dados da Empresa</h6>
                        <div className="mb-3">
                            <CFormLabel htmlFor="inputCompanyName">Nome da Empresa</CFormLabel>
                            <CFormInput
                                type="text"
                                id="inputCompanyName"
                                name="companyName"
                                placeholder="Razão social da empresa"
                                value={formData.companyName}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <CFormLabel htmlFor="inputCnpj">CNPJ</CFormLabel>
                            <CFormInput
                                type="text"
                                id="inputCnpj"
                                name="cnpj"
                                placeholder="00.000.000/0001-00"
                                value={formData.cnpj}
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
