#վ�㹦��ʹ��˵��
## �û�Ȩ�����

* ���ؿ���
    ```
  Web.config�е�AppSettings�м�AccessToken:Enabled ����true������false�ر�
    ```
* �û�Id�Ļ�ȡ
  ```
  Action([UserId] int userId)
  ```

* �û���Ϣ��ȡ
   ```
  Action([UserProfile] UserProfile userProfile)
  ```

* ��ɫ����
 ```
  [Authorize(Roles = "����Ա,�곤")]
  Action(:::)
  ```

* �û����ʿ���
    ```
   [Authorize(Users = "admin,dahongmen")]
   Action(:::)
   ```