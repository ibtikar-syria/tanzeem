 import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44326/',
  redirectUri: baseUrl,
  clientId: 'Tanzeem_App',
  responseType: 'code',
  scope: 'offline_access Tanzeem',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'Tanzeem',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44326',
      rootNamespace: 'Tanzeem',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
