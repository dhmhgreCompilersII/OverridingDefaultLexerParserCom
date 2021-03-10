parser grammar CSVParser;
@header {
	using System;
}

options { tokenVocab = CSVLexer ;}

compileUnit

	:	hdr row* 
	;
	
hdr : row;



row:   field (COMMA field )* SR? NEWLINE
	  | field  (COMMA field )* SR? EOF
	  | SR? NEWLINE
	;

field : TEXT
	  | STRING	  
		;

