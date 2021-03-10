lexer grammar CSVLexer;

SR : '\r';
NEWLINE : '\n';
COMMA : ',';
TEXT : ~[,\n\r"]+;
STRING : '"' ('""'|~'"')* '"';