set curDir=%~dp0
tf get %curDir%/* /force /overwrite /recursive /login:litao,lt123456
tf history %curDir%* /login:litao,lt123456 /recursive