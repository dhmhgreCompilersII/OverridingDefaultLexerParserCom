using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace OverridingDefaultLexerParserCom {
    partial class CSVParser {

        public override IToken Match(int ttype) {
            IToken badToken = this.CurrentToken;
            if (badToken.Type == ttype) {
                if (ttype == -1)
                    this.matchedEOF = true;
                this._errHandler.ReportMatch(this);
                this.Consume();
            } else {
                badToken = this._errHandler.RecoverInline(this);
                if (this._buildParseTrees && badToken.TokenIndex == -1)
                    this._ctx.AddErrorNode(badToken);
            }
            return badToken;
        }

        public override IToken Consume() {
            IToken currentToken = this.CurrentToken;
            if (currentToken.Type != -1)
                this.InputStream.Consume();
            if (this._buildParseTrees | (this._parseListeners != null && (uint)this._parseListeners.Count > 0U)) {
                if (this._errHandler.InErrorRecoveryMode(this)) {
                    IErrorNode node = this._ctx.AddErrorNode(currentToken);
                    if (this._parseListeners != null) {
                        foreach (IParseTreeListener parseListener in (IEnumerable<IParseTreeListener>)this._parseListeners)
                            parseListener.VisitErrorNode(node);
                    }
                } else {
                    ITerminalNode node = this._ctx.AddChild(currentToken);
                    if (this._parseListeners != null) {
                        foreach (IParseTreeListener parseListener in (IEnumerable<IParseTreeListener>)this._parseListeners)
                            parseListener.VisitTerminal(node);
                    }
                }
            }
            return currentToken;
        }
        public virtual void EnterRule([NotNull] ParserRuleContext localctx, int state, int ruleIndex) {
            this.State = state;
            this._ctx = localctx;
            this._ctx.start = this._input.Lt(1);
            if (this._buildParseTrees)
                this.AddContextToParseTree();
            if (this._parseListeners == null)
                return;
            this.TriggerEnterRuleEvent();
        }
    }
}
