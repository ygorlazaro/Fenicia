import { AuthClient } from './auth-client';
import { AxiosResponse } from 'axios';

/**
 * AuthForgotPasswordClient - Handles forgot password operations
 * Implements password reset request and reset functionality
 */
export class AuthForgotPasswordClient extends AuthClient {
  constructor(baseURL?: string) {
    super(baseURL);
  }

  /**
   * Request password reset token
   * POST /forgotpassword
   * @param {string} email - User email
   * @returns {Promise<void>}
   */
  async requestReset(email: string): Promise<void> {
    await this.getClient().post('/forgotpassword', {
      email
    });
  }

  /**
   * Reset password with token
   * POST /forgotpassword/reset
   * @param {Object} resetData - Reset password data
   * @param {string} resetData.email - User email
   * @param {string} resetData.token - Reset token
   * @param {string} resetData.newPassword - New password
   * @returns {Promise<void>}
   */
  async resetPassword(resetData: { email: string; token: string; newPassword: string }): Promise<void> {
    await this.getClient().post('/forgotpassword/reset', {
      email: resetData.email,
      token: resetData.token,
      newPassword: resetData.newPassword
    });
  }
}

export default AuthForgotPasswordClient;
