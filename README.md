# RabbitEnglish
An English vocabulary learning app created as a thesis project\ Приложение для изучения слов английского языка, созданное в качестве дипломного проекта

В самом приложении осуществелено простое храненние данных в  формате JSON, хранит данные о логине, пароле в виде хеша и
индивидуальном коде, также запоминает картинки, которые были просмотрены, и категории,
к которым они принадлежат. Алгоритм формирования ключа – PBKDF2
Данные хранятся исключительно на устройстве, поэтому при очистке приложения весь прогресс пользователь будет утерян.

The application itself has a simple storage of data in JSON format, stores login data, password in the form of a hash and
individual code, also remembers the pictures that have been viewed and the categories
to which they belong. The key formation algorithm is PBKDF2
Data is stored exclusively on the device, so when the application is cleared, all user progress will be lost.
