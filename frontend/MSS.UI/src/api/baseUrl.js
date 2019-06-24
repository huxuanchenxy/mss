let mock = '/mock'
let api = 'http://127.0.0.1:3851/api/v1'
if (process.env.NODE_ENV === 'production') {
    api = ''
}

export {api, mock}
