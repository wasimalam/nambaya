import { OpenIdConfiguration } from 'angular-auth-oidc-client';
import { Addresses } from './Addresses';

export class UserConfig{
    addresses: Addresses = new Addresses();

    pharmacistConfig: OpenIdConfiguration = {
        stsServer: this.addresses.stsServerAddress,
        redirect_url: this.addresses.pharmacistRedirectUrl,
        client_id: 'pharmacist_ui_client',
        response_type: 'id_token token', // 'id_token token' Implicit Flow
        scope: 'openid profile email patient_api pharmacy_api offline_access Pharma',
        post_logout_redirect_uri: 'https://qt-life.nambaya-medical.de/loggedout/pharmacist',
        post_login_route: '/pharmacist/dashboard',
        forbidden_route: '/Forbidden',
        unauthorized_route: '/unauthorized',
        log_console_warning_active: false,
        log_console_debug_active: false,
        max_id_token_iat_offset_allowed_in_seconds: 120
      };

      cardiologistConfig: OpenIdConfiguration = {
        stsServer: this.addresses.stsServerAddress,
        redirect_url: this.addresses.cardiologistRedirectUrl,
        client_id: 'cardiologist_ui_client',
        response_type: 'id_token token', // 'id_token token' Implicit Flow
        scope: 'openid profile email patient_api cardiologist_api offline_access Cardio',
        post_logout_redirect_uri: 'https://qt-life.nambaya-medical.de/loggedout/cardiologist',
        post_login_route: '/cardiologist/dashboard',
        forbidden_route: '/Forbidden',
        unauthorized_route: '/unauthorized',
        log_console_warning_active: false,
        log_console_debug_active: false,
        max_id_token_iat_offset_allowed_in_seconds: 120
      };

      centerGroupConfig: OpenIdConfiguration = {
        stsServer: this.addresses.stsServerAddress,
        redirect_url: this.addresses.centerGroupRedirectUrl,
        client_id: 'central_group_ui_client',
        response_type: 'id_token token', // 'id_token token' Implicit Flow
        scope:
          'openid profile email patient_api pharmacy_api central_group_api cardiologist_api offline_access CentralGroupApp',
        post_logout_redirect_uri: 'https://qt-life.nambaya-medical.de/loggedout/center',
        post_login_route: '/center/dashboard',
        forbidden_route: '/Forbidden',
        unauthorized_route: '/unauthorized',
        log_console_warning_active: false,
        log_console_debug_active: false,
        max_id_token_iat_offset_allowed_in_seconds: 120
      };

      nambayauserConfig: OpenIdConfiguration = {
        stsServer: this.addresses.stsServerAddress,
        redirect_url: this.addresses.nambayaUserRedirectUrl,
        client_id: 'nambayauser_ui_client',
        response_type: 'id_token token', // 'id_token token' Implicit Flow
        scope: 'logging_api openid profile email pharmacy_api central_group_api cardiologist_api nambaya_user_api offline_access User',
        post_logout_redirect_uri: 'https://qt-life.nambaya-medical.de/loggedout/user',
        post_login_route: '/user/dashboard',
        forbidden_route: '/Forbidden',
        unauthorized_route: '/unauthorized',
        log_console_warning_active: false,
        log_console_debug_active: false,
        max_id_token_iat_offset_allowed_in_seconds: 120
      };
}
