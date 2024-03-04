# Compiler Theory

**Compiler Theory** - это приложение для редактирования и анализа кода. Оно предоставляет удобный текстовый редактор с расширенными возможностями, также включает функции анализа кода с поддержкой автодополнения и вывода ошибок компиляции.

## Оглавление

- [Лабораторная работа №1: Разработка пользовательского интерфейса (GUI) для языкового процессора](#лабораторная-работа-1-разработка-пользовательского-интерфейса-gui-для-языкового-процессора)
- [Лабораторная работа №2: Разработка лексического анализатора (сканера)](#лабораторная-работа-2-разработка-лексического-анализатора-сканера)

## Лабораторная работа №1: Разработка пользовательского интерфейса (GUI) для языкового процессора

**Тема:** Создание текстового редактора с возможностью последующего расширения в направлении языкового процессора.

**Цель работы:** Разработка графического приложения с интерфейсом пользователя для редактирования текстовых данных. Приложение предполагается использовать как основу для будущего расширения функционала в направлении языкового процессора.

**Язык программирования:** C#, Avalonia.

## Возможности

### Текстовый Редактор

- **Меню**
- | Пункт меню | Подпункты                            |
    | ------ |--------------------------------------|
  | Файл | ![Файл](screenshots/FileMenu.png)    |
  | Правка | ![Правка](screenshots/EditMenu.png)  |
  | Текст | ![Текст](screenshots/TextMenu.png)   |
  | Справка | ![Справка](screenshots/DocsMenu.png) |

- **Базовые Операции:**
    - Открытие, создание и сохранение файлов.
    - Поддержка множества вкладок для одновременного редактирования различных файлов.

      ![Placeholder Screenshot](screenshots/tabsExample.png)

- **Редактирование Кода:**
    - Подсветка синтаксиса и автодополнение для улучшенной читаемости кода.
    - Возможность отмены (Undo) и повтора (Redo) действий.
    - Операции копирования, вставки, удаления и выделения текста.

      ![Placeholder Screenshot](screenshots/editExample.png)

- **Настройки Внешнего Вида:**
    - Изменение размера шрифта для удобства чтения.

      ![Placeholder Screenshot](screenshots/textSizeExample.png)

### Анализ Кода

- **Ошибки Компиляции:**
    - Вывод ошибок компиляции в удобном формате.

      ![Placeholder Screenshot](screenshots/errorsExample.png)

### Другие Функции

- **Открытие Примеров Кода:**
    - Возможность загрузки примеров кода для изучения и экспериментов.

      ![Placeholder Screenshot](screenshots/textExampleExample.png)

- **Поддержка Многозадачности:**
    - Возможность одновременного редактирования и анализа нескольких файлов.

      ![Placeholder Screenshot](screenshots/concurrencyExample1.png)
      ![Placeholder Screenshot](screenshots/concurrencyExample2.png)

## Используемые Библиотеки

- [ReactiveUI](https://www.nuget.org/packages/ReactiveUI/) (версия 19.5.41)
- [ReactiveUI.Fody](https://www.nuget.org/packages/ReactiveUI.Fody/) (версия 19.5.41)
- [Avalonia](https://www.nuget.org/packages/Avalonia/) (версия 11.0.6)
- [Avalonia.Controls.DataGrid](https://www.nuget.org/packages/Avalonia.Controls.DataGrid/) (версия 11.0.6)
- [AvaloniaEdit](https://www.nuget.org/packages/AvaloniaEdit/) (версия 11.0.6)
- [Material.Avalonia](https://www.nuget.org/packages/Material.Avalonia) (версия 3.4.0)
- [Material.Avalonia.DataGrid](https://www.nuget.org/packages/Material.Avalonia.DataGrid) (версия 3.4.0)
- [Material.Avalonia.Dialogs](https://www.nuget.org/packages/Material.Avalonia.Dialogs) (версия 3.4.0)
- [Material.Icons.Avalonia](https://www.nuget.org/packages/Material.Icons.Avalonia/) (версия 2.1.0)

## Лабораторная работа №2: Разработка лексического анализатора (сканера)

**Тема:** разработка лексического анализатора (сканера).

**Цель работы:** изучить назначение лексического анализатора. Спроектировать алгоритм и выполнить программную реализацию сканера.

| №  | Тема | Пример верной строки | Справка |
|----| ------ | ------ | ------ |
| 17 | Объявление и инициализация строковой константы на языке Rust | const NAME:&str = "GFG"; | [ссылка](https://www.geeksforgeeks.org/rust-constants/) |

**В соответствии с вариантом задания необходимо:**

1. Спроектировать диаграмму состояний сканера.
2. Разработать лексический анализатор, позволяющий выделить в тексте лексемы, иные символы считать недопустимыми (выводить ошибку).
3. Встроить сканер в ранее разработанный интерфейс текстового редактора. Учесть, что текст для разбора может состоять из множества строк.

**Входные данные:** строка (текст программного кода).

**Выходные данные:** последовательность условных кодов, описывающих структуру разбираемого текста с указанием места положения и типа.

### Примеры допустимых строк

```rust
const my_str: &str = "hello world";
```

```rust
const my_str: &str = "";
```

```rust
const my_str: &str = "shielding \" test";
```

### Диаграмма состояний сканера

![Диаграмма состояний сканера](screenshots/diagram.png)

### Тестовые примеры

1. **Тест №1.** Пример, показывающий все возможные лексемы, которые могут быть найдены лексическим анализатором.
   ![Тест 1](screenshots/lab2Example1.png)

2. **Тест №2.** Недопустипый вариант. Недопустимые символы.
   ![Тест 2](screenshots/lab2Example2.png)

2. **Тест №3.** Недопустипый вариант. Незакрытая строка.
   ![Тест 3](screenshots/lab2Example3.png)

## Используемые Библиотеки

- [ReactiveUI](https://www.nuget.org/packages/ReactiveUI/) (версия 19.5.41)
- [ReactiveUI.Fody](https://www.nuget.org/packages/ReactiveUI.Fody/) (версия 19.5.41)
- [Avalonia](https://www.nuget.org/packages/Avalonia/) (версия 11.0.6)
- [Avalonia.Controls.DataGrid](https://www.nuget.org/packages/Avalonia.Controls.DataGrid/) (версия 11.0.6)
- [AvaloniaEdit](https://www.nuget.org/packages/AvaloniaEdit/) (версия 11.0.6)
- [Material.Avalonia](https://www.nuget.org/packages/Material.Avalonia) (версия 3.4.0)
- [Material.Avalonia.DataGrid](https://www.nuget.org/packages/Material.Avalonia.DataGrid) (версия 3.4.0)
- [Material.Avalonia.Dialogs](https://www.nuget.org/packages/Material.Avalonia.Dialogs) (версия 3.4.0)
- [Material.Icons.Avalonia](https://www.nuget.org/packages/Material.Icons.Avalonia/) (версия 2.1.0)

*Примечание: Убедитесь, что все пакеты устанавливаются из указанных версий для обеспечения совместимости.*