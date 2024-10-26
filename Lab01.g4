grammar Lab01;

/*
 * Parser Rules
 */

compileUnit : expression EOF;

expression :
    LPAREN expression RPAREN               #ParenthesizedExpr
    | expression EXPONENT expression        #ExponentialExpr
    | expression operatorToken=(MULTIPLY | DIVIDE | DIV | MOD) expression #MultiplicativeExpr
    | expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
    | NUMBER                               #NumberExpr
    | IDENTIFIER                           #IdentifierExpr
    | cellAddress                          #CellAddressExpr // ������� ��� ������� �����
    ;

cellAddress : COLUMN ROW; // ��������, "A1" ��� "B2"

/*
 * Lexer Rules
 */

NUMBER : INT ('.' INT)?;
IDENTIFIER : [a-zA-Z]+ [1-9][0-9]*;

COLUMN : [A-Z]+; // ���������� ��������� ������������� �������
ROW : [1-9][0-9]*; // ���������� �������� ������������� ������

INT : ('0'..'9')+;

EXPONENT : '^';
MULTIPLY : '*';
DIVIDE : '/';
DIV : 'div';
MOD : 'mod';
SUBTRACT : '-';
ADD : '+';
LPAREN : '(';
RPAREN : ')';

WS : [ \t\r\n] -> channel(HIDDEN);
