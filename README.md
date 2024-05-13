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

## Лабораторная работа №3: Разработка синтаксического анализатора (парсера)

**Тема:** разработка синтаксического анализатора (парсера).

**Цель работы:** изучить назначение синтаксического анализатора, спроектировать алгоритм и выполнить программную реализацию парсера.

| №  | Тема | Пример верной строки | Справка |
|----| ------ | ------ | ------ |
| 17 | Объявление и инициализация строковой константы на языке Rust | const NAME:&str = "GFG"; | [ссылка](https://www.geeksforgeeks.org/rust-constants/) |

[Примеры допустимых строк](#примеры-допустимых-строк)

**В соответствии с вариантом задания на курсовую работу необходимо:**
1. Разработать автоматную грамматику.
2. Спроектировать граф конечного автомата (перейти от автоматной грамматики к конечному автомату).
3. Выполнить программную реализацию алгоритма работы конечного автомата.
4. Встроить разработанную программу в интерфейс текстового редактора, созданного на первой лабораторной работе.

### Грамматика

G[&lt;C&gt; = &lt;строковая константа&gt;]:

V<sub>T</sub> = { 'const', str’, ‘"’, '\', ':', '&', '=', ';', _, ‘a’…’z’, ‘A’…’Z’, ‘0’…’9’}

V<sub>N</sub> = { &lt;C&gt;, I, IT, T, A, S, ST, OE, E }

P = {
1. &lt;C&gt; → ‘const’ I
2. I → '_' IT
3. I → letter IT
4. IT → ‘_’ IT
5. IT → digit IT
6. IT → letter IT
7. IT → ':' T
8. T → '&str' A
9. A → '=' S
10. S → '"' ST
11. ST → symbol ST
12. ST → '\\"' ST
13. ST → '"' OE
14. OE → ';'
15. E → epsilon

}

### Классификация грамматики

Согласно классификации Хомского, грамматика G[Z] является полностью автоматной.

### Граф конечного автомата

![Граф конечного автомата](screenshots/grammar_graph.png)

### Тестовые примеры

1. **Тест №1.** Все выражения написаны корректно.

   ![Тест 1](screenshots/good.png)
2. **Тест №2.** Пример ошибок.

   ![Тест 2](screenshots/test21.png)
3. **Тест №3.** Пример ошибок.

   ![Тест 3](screenshots/test22.png)

## Лабораторная работа №4: Нейтрализация ошибок (метод Айронса)

**Тема**: нейтрализация ошибок (метод Айронса).

**Цель работы:** реализовать алгоритм нейтрализации синтаксических ошибок и дополнить им программную реализацию парсера.

### Метод Айронса

Разрабатываемый синтаксический анализатор построен на базе автоматной грамматики. При нахождении лексемы, которая не соответствует грамматике предлагается свести алгоритм нейтрализации к последовательному
удалению следующего символа во входной цепочке до тех пор, пока следующий символ не окажется одним из допустимых в данный момент разбора.

Этот алгоритм был мной уже реализован в Лабораторной работе №3. В таблице ошибок выводятся их местоположение и текст ошибки, содержащий информацию об отброшенном фрагменте.

### Тестовые примеры

1. **Тест №1.** Пример ошибок.

   ![Тест 1](screenshots/4test11.png)
   ![Тест 1](screenshots/4test12.png)
2. **Тест №2.** Пример ошибок.

   ![Тест 3](screenshots/4test21.png)
   ![Тест 3](screenshots/4test22.png)

## Лабораторная работа №5: Включение семантики в анализатор. Создание внутренней формы представления программы

**Тема:** включение семантики в анализатор, создание внутренней формы представления программы, используя польскую инверсную запись (ПОЛИЗ).

**Цель работы:** дополнить анализатор, разработанный в рамках лабораторных работ, этапом формирования внутренней формы представления программы.

### Тестовые примеры

1. **Тест №1.** Вычисление значения арифметического выражения.

   ![Тест 1 a](screenshots/5test1a.png)
   ![Тест 1 b](screenshots/5test1b.png)
2. **Тест №2.** Обработка деления на 0.

   ![Тест 2 a](screenshots/5test2a.png)
   ![Тест 2 b](screenshots/5test2b.png)
3. **Тест №3.** Обработка недопустимых символов.

   ![Тест 3](screenshots/5test3.png)

## Лабораторная работа №7: Реализация метода рекурсивного спуска для синтаксического анализа

**Тема:** реализация метода рекурсивного спуска для синтаксического анализа.

**Цель работы:** разработать для грамматики алгоритм синтаксического анализа на основе метода рекурсивного спуска.

Для грамматики G[&lt;unsigned_integer&gt;] разработать и реализовать алгоритм анализа на основе метода рекурсивного спуска.

G[&lt;unsigned_integer&gt;]:
1. &lt;unsigned_integer&gt; → &lt;decimal_number&gt; | &lt;exponential_part&gt;| &lt;decimal_number&gt;&lt;exponential_part&gt;;
2. &lt;decimal_number&gt; → &lt;unsigned_integer&gt; | &lt;decimal_fraction&gt;| &lt;unsigned_integer&gt;&lt;decimal_fraction&gt;;
3. &lt;unsigned_integer&gt; → &lt;digit&gt; | &lt;unsigned_integer&gt;&lt;digit&gt;;
4. &lt;integer&gt; → &lt;unsigned_integer&gt; | +&lt;unsigned_integer&gt;| -&lt;unsigned_integer&gt;;
5. &lt;decimal_fraction&gt; → .&lt;unsigned_integer&gt;;
6. &lt;exponential_part&gt; → 10&lt;integer&gt;;
6. &lt;digit&gt; → 0 | 1 | ... | 9;

По классификации Хомского данная грамматика относится к КС.

### Тестовые примеры

```
123.1111 10+5
```

![Тест 1](screenshots/test7.png)
