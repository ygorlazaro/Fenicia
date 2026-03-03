import React, { useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
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
    CRow,
    CAlert
} from "@coreui/react";
import AuthForgotPasswordClient from '../../../services/auth-forgot-password-client';

const forgotPasswordClient = new AuthForgotPasswordClient("http://localhost:5144");

const ResetPassword = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    
    const [formData, setFormData] = useState({
        email: searchParams.get('email') || '',
        token: searchParams.get('token') || '',
        newPassword: '',
        confirmPassword: ''
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);

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

        // Validate passwords match
        if (formData.newPassword !== formData.confirmPassword) {
            setError('As senhas não coincidem.');
            setLoading(false);
            return;
        }

        // Validate password length
        if (formData.newPassword.length < 6) {
            setError('A senha deve ter pelo menos 6 caracteres.');
            setLoading(false);
            return;
        }

        try {
            await forgotPasswordClient.resetPassword({
                email: formData.email,
                token: formData.token,
                newPassword: formData.newPassword
            });
            setSuccess(true);
            
            // Redirect to login after 3 seconds
            setTimeout(() => {
                navigate('/auth/login');
            }, 3000);
        } catch (err) {
            console.error('Reset password failed:', err);
            setError(err.response?.data?.title || 'Falha ao redefinir senha. O token pode ter expirado.');
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
                            <strong>Redefinir Senha</strong>
                        </CCardHeader>
                        <CCardBody>
                            {error && (
                                <CAlert color="danger" dismissible>
                                    {error}
                                </CAlert>
                            )}

                            {success && (
                                <CAlert color="success" dismissible>
                                    <strong>Senha redefinida com sucesso!</strong> Você será redirecionado para o login.
                                </CAlert>
                            )}

                            {!success && (
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
                                            disabled
                                        />
                                    </div>
                                    <div className="mb-3">
                                        <CFormLabel htmlFor="inputToken">Token</CFormLabel>
                                        <CFormInput
                                            type="text"
                                            id="inputToken"
                                            name="token"
                                            placeholder="Token de recuperação"
                                            value={formData.token}
                                            onChange={handleInputChange}
                                            required
                                        />
                                    </div>
                                    <div className="mb-3">
                                        <CFormLabel htmlFor="inputNewPassword">Nova Senha</CFormLabel>
                                        <CFormInput
                                            type="password"
                                            id="inputNewPassword"
                                            name="newPassword"
                                            placeholder="Nova senha"
                                            value={formData.newPassword}
                                            onChange={handleInputChange}
                                            required
                                            minLength={6}
                                        />
                                    </div>
                                    <div className="mb-3">
                                        <CFormLabel htmlFor="inputConfirmPassword">Confirmar Senha</CFormLabel>
                                        <CFormInput
                                            type="password"
                                            id="inputConfirmPassword"
                                            name="confirmPassword"
                                            placeholder="Confirme a nova senha"
                                            value={formData.confirmPassword}
                                            onChange={handleInputChange}
                                            required
                                            minLength={6}
                                        />
                                    </div>
                                    <div className="col-auto">
                                        <CButton
                                            color="primary"
                                            type="submit"
                                            className="mb-3"
                                            disabled={loading}
                                        >
                                            {loading ? 'Redefinindo...' : 'Redefinir senha'}
                                        </CButton>
                                    </div>
                                </CForm>
                            )}

                            <div className="mt-3">
                                <Link to="/auth/login">
                                    <CButton color="link" className="p-0">
                                        Voltar para o login
                                    </CButton>
                                </Link>
                            </div>
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        </CContainer>
    )
};

export default ResetPassword;
