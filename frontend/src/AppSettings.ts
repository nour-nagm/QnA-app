export const server = 'http://localhost:5000';

export const webAPIUrl = `${server}/api`;

export const authSettings = {
  domain: 'dev-ougaydzj.us.auth0.com',
  client_id: 'EwnLSZRnW4ZBHJpjqTW68wQnq1pfFZTU',
  redirect_uri: window.location.origin + '/signin-callback',
  scope: 'openid profile QandAAPI email',
  audience: 'https://qna',
};
