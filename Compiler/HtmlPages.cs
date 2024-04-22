namespace Compiler;

public static class HtmlPages
{
    public static string About =
        """<!DOCTYPE html>\n<html lang=\"ru\">\n<head>\n   <meta charset=\"UTF-8\">\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\n    <title>О программе</title>\n    <style>\n        body {\n            font-family: Arial, sans-serif;\n            margin: 0;\n            padding: 0;\n            display: flex;\n            justify-content: center;\n            align-items: center;\n            height: 100vh;\n            background-color: #f4f4f4;\n        }\n\n        .container {\n            text-align: center;\n            max-width: 600px;\n            padding: 20px;\n            border: 1px solid #ccc;\n            border-radius: 10px;\n            background-color: #fff;\n            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\n        }\n\n        h1 {\n            color: #333;\n            margin-bottom: 20px;\n        }\n\n        p {\n            color: #555;\n            margin-bottom: 10px;\n            line-height: 1.4;\n        }\n\n        .author {\n            font-weight: bold;\n            color: #007BFF;\n        }\n\n        .version {\n            color: #28A745;\n        }\n\n        .description {\n            font-style: italic;\n            color: #6C757D;\n        }\n\n        .year {\n            color: #FFC107;\n        }\n\n        .university {\n            color: #6610f2;\n            font-weight: bold;\n            margin-top: 20px;\n        }\n    </style>\n</head>\n<body>\n<div class=\"container\">\n    <h1>О программе</h1>\n    <p class=\"version\">Версия программы: 1.0.0</p>\n    <p class=\"author\">Автор: Чуриков Д. И., АВТ-113</p>\n    <p class=\"description\">Описание: текстовый редактор с функциями языкового процессора</p>\n    <p class=\"year\">2024 г.</p>\n    <p class=\"university\">Разработано для Новосибирского государственного технического университета (НГТУ)</p>\n</div>\n</body>\n</html>\n""";

    public static string FormulationOfTheProblem =
        """<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Постановка задачи</title></head><body><h1>Постановка задачи</h1><h2>Тема работы</h2><p>Создание текстового редактора с возможностью последующего расширения в направлении языкового процессора.</p><h2>Цель работы:</h2><p>Разработка графического приложения с интерфейсом пользователя для редактирования текстовых данных. Приложение предполагается использовать как основу для будущего расширения функционала в направлении языкового процессора.</p><h2>Возможности:</h2><ul><li>Открытие, создание и сохранение файлов.</li><li>Поддержка множества вкладок для одновременного редактирования различных файлов.</li><li>Подсветка синтаксиса и автодополнение для улучшенной читаемости кода.</li><li>Возможность отмены (Undo) и повтора (Redo) действий.</li><li>Операции копирования, вставки, удаления и выделения текста.</li><li>Изменение размера шрифта для удобства чтения.</li><li>Вывод ошибок компиляции в удобном формате.</li><li>Возможность загрузки примеров кода для изучения и экспериментов.</li><li>Возможность одновременного редактирования и анализа нескольких файлов.</li></ul><h2>Примеры верных строк:</h2><code>const my_str: &str = "hello world";</code><br><code>const my_str: &str = "";</code></body></html>""";

    public static string Grammar =
        """<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Грамматика</title><style>table { border-collapse: collapse; } th, td { border: 1px solid black; padding: 8px; }</style></head><body><h1>Грамматика</h1><table><tr><th>Нетерминал</th><th>Продукция</th></tr><tr><td>C</td><td>'const' I</td></tr><tr><td>I</td><td>'_' IT</td></tr><tr><td>I</td><td>letter IT</td></tr><tr><td>IT</td><td>'_' IT</td></tr><tr><td>IT</td><td>digit IT</td></tr><tr><td>IT</td><td>letter IT</td></tr><tr><td>IT</td><td>':' T</td></tr><tr><td>T</td><td>'&str' A</td></tr><tr><td>A</td><td>'=' S</td></tr><tr><td>S</td><td>'"' ST</td></tr><tr><td>ST</td><td>symbol ST</td></tr><tr><td>ST</td><td>'\"' ST</td></tr><tr><td>ST</td><td>OE</td></tr><tr><td>OE</td><td>';'</td></tr><tr><td>E</td><td>epsilon</td></tr></table></body></html>""";

    public static string GrammarClassification =
        """<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Классификация грамматики</title><style>table { border-collapse: collapse; } th, td { border: 1px solid black; padding: 8px; }</style></head><body><h1>Классификация грамматики</h1><p>Согласно классификации Хомского, грамматика G[Z] является полностью автоматной.</p></body></html>""";

    public static string AnalysisMethod =
        """<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Метод анализа</title><style>table { border-collapse: collapse; } th, td { border: 1px solid black; padding: 8px; }</style></head><body><h1>Метод анализа</h1><p></p></body></html>""";

    public static string DiagnosticAndErrorNeutralization =
        """<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Нейрализация ошибок</title><style>table { border-collapse: collapse; } th, td { border: 1px solid black; padding: 8px; }</style></head><body><h1>Нейтрализация ошибок</h1><p>Разрабатываемый синтаксический анализатор построен на базе автоматной грамматики. При нахождении лексемы, которая не соответствует грамматике предлагается свести алгоритм нейтрализации к последовательному удалению следующего символа во входной цепочке до тех пор, пока следующий символ не окажется одним из допустимых в данный момент разбора.</p><p>Этот алгоритм был мной уже реализован в Лабораторной работе №3. В таблице ошибок выводятся их местоположение и текст ошибки, содержащий информацию об отброшенном фрагменте.</p></body></html>""";

    public static string SourcesList =
        """<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Источники</title><style>table { border-collapse: collapse; } th, td { border: 1px solid black; padding: 8px; }</style></head><body><h1>Источники</h1><p></p></body></html>""";

    public static string SourceCodeUrl = "https://github.com/Danilka108/Compiler";
}