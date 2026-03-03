import React, { useState } from 'react';
import { Link } from 'react-router-dom';
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

const ForgotPassword = () => {
    const [email, setEmail] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setSuccess(false);

        try {
            await forgotPasswordClient.requestReset(email);
            setSuccess(true);
        } catch (err) {
            console.error('Forgot password failed:', err);
            setError(err.response?.data?.title || 'Falha ao solicitar recuperação de senha. Verifique seu e-mail.');
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
                            <strong>Recuperar Senha</strong>
                        </CCardHeader>
                        <CCardBody>
                            {error && (
                                <CAlert color="danger" dismissible>
                                    {error}
                                </CAlert>
                            )}

                            {success && (
                                <CAlert color="success" dismissible>
                                    <strong>Sucesso!</strong> Enviamos um link de recuperação para o seu e-mail.
                                    Verifique sua caixa de entrada e siga as instruções.
                                </CAlert>
                            )}

                            {!success && (
                                <>
                                    <p className="text-muted">
                                        Digite seu e-mail cadastrado e enviaremos um link para redefinir sua senha.
                                    </p>
                                    <CForm onSubmit={handleSubmit}>
                                        <div className="mb-3">
                                            <CFormLabel htmlFor="inputEmail">E-mail</CFormLabel>
                                            <CFormInput
                                                type="email"
                                                id="inputEmail"
                                                name="email"
                                                placeholder="name@example.com"
                                                value={email}
                                                onChange={(e) => setEmail(e.target.value)}
                                                required
                                            />
                                        </div>
                                        <div className="col-auto">
                                            <CButton
                                                color="primary"
                                                type="submit"
                                                className="mb-3"
                                                disabled={loading}
                                            >
                                                {loading ? 'Enviando...' : 'Enviar link de recuperação'}
                                            </CButton>
                                        </div>
                                    </CForm>
                                </>
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

export default ForgotPassword;
