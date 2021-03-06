﻿## Roslynator Analyzers by Category

 Category | Title | Id | Enabled by Default 
 --- | --- | --- |:---:
Design|Abstract type should not have public constructors|RCS1160|x
Design|Avoid empty catch clause that catches System\.Exception|RCS1075|x
Design|Avoid locking on publicly accessible instance|RCS1059|x
Design|Avoid static members in generic types|RCS1158|x
Design|Call 'ConfigureAwait\(false\)'|RCS1090|x
Design|Composite enum value contains undefined flag|RCS1157|x
Design|Declare enum member with zero value \(when enum has FlagsAttribute\)|RCS1135|x
Design|Declare type inside namespace|RCS1110|x
Design|Mark class as static|RCS1102|x
ErrorFix|Add break statement to switch section|RCS1116|x
ErrorFix|Add missing semicolon|RCS1122|
ErrorFix|Add return statement that returns default value|RCS1117|x
ErrorFix|Mark containing class as abstract|RCS1144|x
ErrorFix|Mark member as static|RCS1125|x
ErrorFix|Member type must match overriden member type|RCS1152|x
ErrorFix|Remove implementation from abstract member|RCS1149|x
ErrorFix|Remove inapplicable modifier|RCS1147|x
ErrorFix|Replace return with yield return|RCS1131|x
ErrorFix|Replace yield/return statement with expression statement|RCS1115|x
Formatting|Add empty line after closing brace|RCS1153|
Formatting|Add empty line after embedded statement|RCS1030|
Formatting|Add empty line after last statement in do statement|RCS1092|
Formatting|Add empty line between declarations|RCS1057|x
Formatting|Format accessor list|RCS1024|x
Formatting|Format binary operator on next line|RCS1029|x
Formatting|Format declaration braces|RCS1076|x
Formatting|Format documentation summary on a single line|RCS1100|
Formatting|Format documentation summary on multiple lines|RCS1101|
Formatting|Format each enum member on a separate line|RCS1025|
Formatting|Format each statement on a separate line|RCS1026|
Formatting|Format embedded statement on a separate line|RCS1027|
Formatting|Format empty block|RCS1023|x
Formatting|Format switch section's statement on a separate line|RCS1028|
General|Bitwise operation on enum without Flags attribute|RCS1130|x
General|Mark local variable as const|RCS1118|x
General|Remove unreachable code|RCS1148|x
General|Throwing of new NotImplementedException|RCS1079|x
General|Use "" instead of string\.Empty|RCS1078|
General|Use carriage return \+ linefeed as newline|RCS1087|
General|Use linefeed as newline|RCS1086|
General|Use space\(s\) instead of tab|RCS1088|
Maintainability|Add documentation comment to publicly visible type or member|RCS1137|x
Maintainability|Add exception to documentation comment|RCS1140|x
Maintainability|Add parameter to documentation comment|RCS1141|x
Maintainability|Add summary element to documentation comment|RCS1139|x
Maintainability|Add summary to documentation comment|RCS1138|x
Maintainability|Add type parameter to documentation comment|RCS1142|x
Maintainability|Declare each type in separate file|RCS1060|
Maintainability|Remove original exception from throw statement|RCS1044|x
Maintainability|Use nameof operator|RCS1015|x
Naming|Asynchronous method name should end with 'Async'|RCS1046|
Naming|Non\-asynchronous method name should not end with 'Async'|RCS1047|x
Naming|Rename private field according to camel case with underscore|RCS1045|
Performance|Call 'Find' method instead of 'FirstOrDefault' method|RCS1119|x
Performance|Use \[\] instead of calling 'ElementAt'|RCS1120|x
Performance|Use \[\] instead of calling 'First'|RCS1121|x
Performance|Use 'Any' method instead of 'Count' method|RCS1083|x
Performance|Use bitwise operation instead of calling 'HasFlag'|RCS1096|x
Performance|Use 'Count/Length' property instead of 'Any' method|RCS1080|x
Performance|Use 'Count/Length' property instead of 'Count' method|RCS1082|x
Readability|Add default access modifier|RCS1018|x
Readability|Add parentheses according to operator precedence|RCS1123|x
Readability|Add static modifier to all partial class declarations|RCS1108|x
Readability|Avoid chain of assignments|RCS1162|
Readability|Avoid implicitly\-typed array|RCS1014|
Readability|Avoid 'null' on the left side of a binary expression|RCS1098|x
Readability|Avoid usage of using alias directive|RCS1056|
Readability|Declare each attribute separately|RCS1052|
Readability|Declare using directive on top level|RCS1094|
Readability|Default label should be last label in switch section|RCS1099|x
Readability|Enum member should declare explicit value|RCS1161|x
Readability|Reorder modifiers|RCS1019|
Readability|Sort enum members|RCS1154|x
Readability|Split variable declaration|RCS1081|
Readability|Use explicit type instead of 'var' \(even if the type is obvious\)|RCS1012|
Readability|Use explicit type instead of 'var' \(foreach variable\)|RCS1009|x
Readability|Use explicit type instead of 'var' \(when the type is not obvious\)|RCS1008|x
Redundancy|Avoid interpolated string with no interpolation|RCS1062|x
Redundancy|Avoid semicolon at the end of declaration|RCS1055|x
Redundancy|Remove empty attribute argument list|RCS1039|x
Redundancy|Remove empty destructor|RCS1106|x
Redundancy|Remove empty else clause|RCS1040|
Redundancy|Remove empty finally clause|RCS1066|x
Redundancy|Remove empty initializer|RCS1041|x
Redundancy|Remove empty namespace declaration|RCS1072|x
Redundancy|Remove empty region|RCS1091|x
Redundancy|Remove empty statement|RCS1038|x
Redundancy|Remove enum default underlying type|RCS1042|x
Redundancy|Remove file with no code|RCS1093|x
Redundancy|Remove partial modifier from type with a single part|RCS1043|x
Redundancy|Remove redundant 'as' operator|RCS1145|x
Redundancy|Remove redundant base constructor call|RCS1071|x
Redundancy|Remove redundant boolean literal|RCS1033|x
Redundancy|Remove redundant braces|RCS1031|x
Redundancy|Remove redundant cast|RCS1151|x
Redundancy|Remove redundant comma in initializer|RCS1035|x
Redundancy|Remove redundant constructor|RCS1074|x
Redundancy|Remove redundant continue statement|RCS1134|x
Redundancy|Remove redundant default switch section|RCS1070|x
Redundancy|Remove redundant delegate creation|RCS1114|x
Redundancy|Remove redundant Dispose/Close call|RCS1133|x
Redundancy|Remove redundant empty line|RCS1036|x
Redundancy|Remove redundant field initalization|RCS1129|x
Redundancy|Remove redundant overriding member|RCS1132|x
Redundancy|Remove redundant parentheses|RCS1032|x
Redundancy|Remove redundant sealed modifier|RCS1034|x
Redundancy|Remove redundant 'ToCharArray' call|RCS1107|x
Redundancy|Remove redundant 'ToString' call|RCS1097|x
Redundancy|Remove trailing white\-space|RCS1037|x
Redundancy|Remove unnecessary case label|RCS1069|x
Simplification|Call string\.Concat instead of string\.Join|RCS1150|x
Simplification|Combine 'Enumerable\.Where' method chain|RCS1112|x
Simplification|Inline local variable|RCS1124|x
Simplification|Merge else clause with nested if statement|RCS1006|x
Simplification|Merge if statement with nested if statement|RCS1061|x
Simplification|Merge interpolation into interpolated string|RCS1105|x
Simplification|Merge local declaration with initialization|RCS1127|x
Simplification|Merge local declaration with return statement|RCS1054|x
Simplification|Merge switch sections with equivalent content|RCS1136|x
Simplification|Replace if statement with assignment|RCS1103|x
Simplification|Replace if statement with return statement|RCS1073|
Simplification|Simplify boolean comparison|RCS1049|x
Simplification|Simplify coalesce expression|RCS1143|x
Simplification|Simplify conditional expression|RCS1104|x
Simplification|Simplify lambda expression parameter list|RCS1022|
Simplification|Simplify lambda expression|RCS1021|x
Simplification|Simplify LINQ method chain|RCS1077|x
Simplification|Simplify logical not expression|RCS1068|x
Simplification|Simplify nested using statement|RCS1005|x
Simplification|Simplify Nullable\<T\> to T?|RCS1020|x
Simplification|Use auto\-implemented property instead of expanded property|RCS1085|x
Simplification|Use 'Cast' method instead of 'Select' method|RCS1109|x
Simplification|Use coalesce expression instead of conditional expression|RCS1084|x
Simplification|Use coalesce expression|RCS1128|x
Simplification|Use compound assignment|RCS1058|x
Simplification|Use postfix unary operator instead of assignment|RCS1089|x
Simplification|Use 'var' instead of explicit type \(when the type is obvious\)|RCS1010|x
Style|Add braces to if\-else|RCS1003|x
Style|Add braces to switch section with multiple statements|RCS1111|
Style|Add braces|RCS1001|x
Style|Add constructor argument list|RCS1050|
Style|Avoid embedded statement in if\-else|RCS1126|
Style|Avoid embedded statement|RCS1007|
Style|Avoid multiline expression body|RCS1017|
Style|Avoid usage of do statement to create an infinite loop|RCS1063|x
Style|Avoid usage of for statement to create an infinite loop|RCS1064|
Style|Avoid usage of while statement to create an inifinite loop|RCS1065|
Style|Parenthesize condition in conditional expression|RCS1051|
Style|Remove braces from if\-else|RCS1004|
Style|Remove braces|RCS1002|
Style|Remove empty argument list|RCS1067|
Usage|Use C\# 6\.0 dictionary initializer|RCS1095|x
Usage|Use conditional access|RCS1146|x
Usage|Use EventHandler\<T\>|RCS1159|x
Usage|Use expression\-bodied member|RCS1016|
Usage|Use lambda expression instead of anonymous method|RCS1048|x
Usage|Use predefined type|RCS1013|
Usage|Use 'string\.IsNullOrEmpty' method|RCS1113|x
Usage|Use string\.Length instead of comparison with empty string|RCS1156|x
Usage|Use StringComparison when comparing strings|RCS1155|x
