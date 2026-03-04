import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
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
import AuthForgotPasswordClient from '../../../services/auth-forgot-password-client';

const forgotPasswordClient = new AuthForgotPasswordClient("http://localhost:5144");

const ForgotPassword = () => {
    const { t } = useTranslation();
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
            setError(err.response?.data?.title || t('auth.forgotPassword.errors.requestFailed', 'Falha ao solicitar recuperação de senha. Verifique seu e-mail.'));
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthLayout>
            <CCard className="mb-4 shadow-sm">
                <CCardHeader className="bg-primary text-white">
                    <strong>{t('auth.forgotPassword.title', 'Recuperar Senha')}</strong>
                </CCardHeader>
                <CCardBody>
                    {error && (
                        <CAlert color="danger" dismissible onClose={() => setError(null)}>
                            {error}
                        </CAlert>
                    )}

                    {success && (
                        <CAlert color="success" dismissible onClose={() => setSuccess(null)}>
                            <strong>{t('auth.forgotPassword.success.title', 'Sucesso!')}</strong> {t('auth.forgotPassword.success.message', 'Enviamos um link de recuperação para o seu e-mail. Verifique sua caixa de entrada e siga as instruções.')}
                        </CAlert>
                    )}

                    {!success && (
                        <>
                            <p className="text-muted">
                                {t('auth.forgotPassword.instructions', 'Digite seu e-mail cadastrado e enviaremos um link para redefinir sua senha.')}
                            </p>
                            <CForm onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <CFormLabel htmlFor="inputEmail">{t('auth.labels.email', 'E-mail')}</CFormLabel>
                                    <CFormInput
                                        type="email"
                                        id="inputEmail"
                                        name="email"
                                        placeholder={t('auth.placeholders.email', 'name@example.com')}
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="d-grid gap-2">
                                    <CButton
                                        color="primary"
                                        type="submit"
                                        disabled={loading}
                                    >
                                        {loading ? t('auth.forgotPassword.buttons.sending', 'Enviando...') : t('auth.forgotPassword.buttons.sendLink', 'Enviar link de recuperação')}
                                    </CButton>
                                </div>
                            </CForm>
                        </>
                    )}

                    <div className="text-center mt-3">
                        <Link to="/auth/login" className="text-decoration-none">
                            {t('auth.forgotPassword.links.backToLogin', 'Voltar para o login')}
                        </Link>
                    </div>
                </CCardBody>
            </CCard>
        </AuthLayout>
    )
};

export default ForgotPassword;
