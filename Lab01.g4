grammar Lab01;

/*
 * Parser Rules
 */

compileUnit : expression EOF;

expression :
    LPAREN expression RPAREN                              #ParenthesizedExpr
    | NOT expression                                      #NotExpr
    | expression EXPONENT expression                      #ExponentialExpr
    | expression operatorToken=(MULTIPLY | DIVIDE | DIV | MOD) expression #MultiplicativeExpr
    | expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
    | expression operatorToken=(EQUAL | LESS | GREATER) expression #ComparisonExpr
    | NUMBER                                              #NumberExpr
    | IDENTIFIER                                          #IdentifierExpr
    | cellAddress                                         #CellAddressExpr
    ;

cellAddress : COLUMN ROW; // например, "A1" или "B2"

/*
 * Lexer Rules
 */

NUMBER : INT ('.' INT)?;
IDENTIFIER : [a-zA-Z]+ [1-9][0-9]*;

COLUMN : [A-Z]+;
ROW : [1-9][0-9]*;

INT : ('0'..'9')+;

EXPONENT : '^';
MULTIPLY : '*';
DIVIDE : '/';
DIV : 'div';
MOD : 'mod';
SUBTRACT : '-';
ADD : '+';
EQUAL : '=';
LESS : '<';
GREATER : '>';
NOT : 'not';
LPAREN : '(';
RPAREN : ')';

WS : [ \t\r\n] -> channel(HIDDEN);
