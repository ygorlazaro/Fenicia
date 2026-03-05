import {
    CAlert,
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CForm,
    CFormInput,
    CFormLabel
} from "@coreui/react";
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';
import AuthLayout from 'src/components/AuthLayout';
import AuthTokenClient from '../../../services/auth-token-client';

const authClient = new AuthTokenClient("http://localhost:5144");

const AuthLogin = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        email: '',
        password: ''
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
            await authClient.generateToken({
                email: formData.email,
                password: formData.password
            });

            navigate('/auth/company');
        } catch (err) {
            console.error('Login failed:', err);
            setError(err.response?.data?.title || t('auth.login.errors.authenticationFailed', 'Falha ao autenticar. Verifique suas credenciais.'));
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthLayout>
            <CCard className="mb-4 shadow-sm">
                <CCardHeader className="bg-primary text-white">
                    <strong>{t('auth.login.title', 'Login')}</strong>
                </CCardHeader>
                <CCardBody>
                    {error && (
                        <CAlert color="danger" dismissible onClose={() => setError(null)}>
                            {error}
                        </CAlert>
                    )}
                    <CForm onSubmit={handleSubmit}>
                        <div className="mb-3">
                            <CFormLabel htmlFor="inputEmail">{t('auth.login.labels.email', 'E-mail')}</CFormLabel>
                            <CFormInput
                                type="email"
                                id="inputEmail"
                                name="email"
                                placeholder={t('auth.login.placeholders.email', 'name@example.com')}
                                value={formData.email}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <CFormLabel htmlFor="inputPassword">{t('auth.login.labels.password', 'Senha')}</CFormLabel>
                            <CFormInput
                                type="password"
                                id="inputPassword"
                                name="password"
                                placeholder={t('auth.login.placeholders.password', 'senha')}
                                value={formData.password}
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
                                {loading ? t('auth.login.buttons.loggingIn', 'Entrando...') : t('auth.login.buttons.login', 'Entrar')}
                            </CButton>
                        </div>

                        <div className="text-center mt-3">
                            <Link to="/auth/register" className="d-block mb-2">
                                <CButton color="secondary" className="w-100">
                                    {t('auth.login.buttons.createAccount', 'Criar conta')}
                                </CButton>
                            </Link>
                            <Link to="/auth/forgot-password" className="text-decoration-none">
                                {t('auth.login.links.forgotPassword', 'Esqueceu a senha?')}
                            </Link>
                        </div>
                    </CForm>
                </CCardBody>
            </CCard>
        </AuthLayout>
    )
};

export default AuthLogin;
