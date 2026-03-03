import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import {
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CContainer,
    CForm,
    CFormInput,
    CFormLabel,
    CRow
} from "@coreui/react";
import AuthTokenClient from '../../../services/auth-token-client';

const authClient = new AuthTokenClient("http://localhost:5144");

const AuthLogin = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        email: '',
        password: '',
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
            const response = await authClient.generateToken({
                email: formData.email,
                password: formData.password,
                cnpj: formData.cnpj
            });

            // Redirect to company selection
            navigate('/auth/company');
        } catch (err) {
            console.error('Login failed:', err);
            setError(err.response?.data?.title || 'Falha ao autenticar. Verifique suas credenciais.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <CContainer className="my-auto">
            <CRow className="justify-content-center">
                <CCol md={6} lg={5}>
                    <CCard className="mb-4">
                    <CCardHeader>
                        <strong>Login</strong>
                    </CCardHeader>
                    <CCardBody>
                        {error && (
                            <div className="alert alert-danger" role="alert">
                                {error}
                            </div>
                        )}
                        <CForm onSubmit={handleSubmit}>
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
                                />
                            </div>
                            <div className="mb-3">
                                <CFormLabel htmlFor="inputCnpj">CNPJ</CFormLabel>
                                <CFormInput
                                    type="text"
                                    id="inputCnpj"
                                    name="cnpj"
                                    placeholder="00000000000100"
                                    value={formData.cnpj}
                                    onChange={handleInputChange}
                                />
                            </div>
                            <div className="col-auto">
                                <CButton 
                                    color="primary" 
                                    type="submit" 
                                    className="mb-3"
                                    disabled={loading}
                                >
                                    {loading ? 'Entrando...' : 'Entrar'}
                                </CButton>
                            </div>

                            <div className="col-auto">
                                <Link to="/auth/register">
                                    <CButton color="secondary" className="mb-3">
                                        Criar conta
                                    </CButton>
                                </Link>
                            </div>

                            <div className="col-auto">
                                <Link to="/auth/forgot-password">
                                    <CButton color="link" className="mb-3">
                                        Esqueceu a senha?
                                    </CButton>
                                </Link>
                            </div>
                        </CForm>
                    </CCardBody>
                </CCard>
                </CCol>
            </CRow>
        </CContainer>
    )
};

export default AuthLogin;