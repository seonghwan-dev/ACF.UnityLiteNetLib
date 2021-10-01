# ACF.UnityLiteNetLib
ACF 작업을 위한 LiteNetLib 구현 간단 테스트 프로젝트 (UDP Chat)
Simple LiteNetLib implementation test project for ACF(My Private Project).

## QuickStart
1. clone repository
2. open project with compatible unity editor
3. modify command line options in `Assets/Editor Default Resources`.
  - you can edit this in `Project Settings/Command Line`
  - this options for editor only.
4. Build
5. Create `client.bat` and `server.bat`

### server.bat
```sh
Uber\LiteNetChat.exe --scene=1 --serverPort=29320
```

### client.bat
```sh
Build\LiteNetChat.exe --scene=2 --address=127.0.0.1 --port=29400 --serverPort=29320
```

- `-scene` : the index of being loaded scene.  
- `-address` : server ip address ex) 123.45.678.xxx  
- `-port` : client listening port  
- `-serverPort` : server listening port  

![image](https://user-images.githubusercontent.com/79823287/135565993-a9ec01aa-7f35-44f7-a31b-b1699518944e.png)
