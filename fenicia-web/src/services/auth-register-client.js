import { AuthClient } from './auth-client';

/**
 * AuthRegisterClient - Handles user registration operations
 * Implements user registration functionality
 */
export class AuthRegisterClient extends AuthClient {
  constructor(baseURL) {
    super(baseURL);
  }

  /**
   * Register a new user
   * POST /register
   * @param {Object} userData - User registration data
   * @param {string} userData.email - User email
   * @param {string} userData.password - User password
   * @param {string} userData.name - User name
   * @param {Object} userData.company - Company data
   * @param {string} userData.company.cnpj - Company CNPJ
   * @param {string} userData.company.name - Company name
   * @param {string} [userData.company.timeZone] - Company time zone
   * @returns {Promise<CreateNewUserResponse>}
   */
  async register(userData) {
    const response = await this.getClient().post('/register', {
      email: userData.email,
      password: userData.password,
      name: userData.name,
      company: {
        cnpj: userData.company.cnpj,
        name: userData.company.name,
        timeZone: userData.company.timeZone
      }
    });

    return response.data;
  }
}

export default AuthRegisterClient;
