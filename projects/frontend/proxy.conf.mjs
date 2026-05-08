const PROXY_CONFIG = {
  '/api': {
    target: 'http://localhost:5010',
    secure: false,
    changeOrigin: true,
  },
};

export default PROXY_CONFIG;
