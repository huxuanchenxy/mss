import axios from 'axios'
import MockAdapter from 'axios-mock-adapter'
import { MenuList } from './data/menuData'

export default {
    /**
   * mock bootstrap
   */
    bootstrap () {
        let mock = new MockAdapter(axios)
        // mock success request
        mock.onAny('/success').reply(200, {
            msg: 'success'
        })

        // mock error request
        mock.onAny('/error').reply(500, {
            msg: 'failure'
        })

        // 获取菜单
        mock.onAny('/mock/UserInfo/GetMenu').reply(200, {
            code: 'SUCESS',
            queryList: MenuList
        })

        // 获取用户
        mock.onAny('/mock/users/all').reply(200, {
            code: 'SUCESS',
            data: [{id:1, name:'test1'},
                { id: 2, name: 'test2' }]
        })

        const api = new RegExp("/api/*");
        mock.onAny(api).passThrough();
    }
}
