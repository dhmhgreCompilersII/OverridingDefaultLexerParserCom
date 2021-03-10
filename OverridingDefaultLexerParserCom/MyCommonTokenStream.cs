using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;

namespace OverridingDefaultLexerParserCom {

    public class MyBufferedTokenStream : ITokenStream, IIntStream {
        /// <summary>
        /// The
        /// <see cref="T:Antlr4.Runtime.ITokenSource" />
        /// from which tokens for this stream are fetched.
        /// </summary>
        [NotNull]
        protected internal ITokenSource tokenSource;
        /// <summary>A collection of all tokens fetched from the token source.</summary>
        /// <remarks>
        /// A collection of all tokens fetched from the token source. The list is
        /// considered a complete view of the input once
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.fetchedEOF" />
        /// is set
        /// to
        /// <see langword="true" />
        /// .
        /// </remarks>
        protected internal IList<IToken> tokens = (IList<IToken>)new List<IToken>(100);
        /// <summary>
        /// The index into
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.tokens" />
        /// of the current token (next token to
        /// <see cref="M:Antlr4.Runtime.BufferedTokenStream.Consume" />
        /// ).
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.tokens" />
        /// <c>[</c>
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.p" />
        /// <c>]</c>
        /// should be
        /// <see cref="M:Antlr4.Runtime.BufferedTokenStream.Lt(System.Int32)">LT(1)</see>
        /// .
        /// <p>This field is set to -1 when the stream is first constructed or when
        /// <see cref="M:Antlr4.Runtime.BufferedTokenStream.SetTokenSource(Antlr4.Runtime.ITokenSource)" />
        /// is called, indicating that the first token has
        /// not yet been fetched from the token source. For additional information,
        /// see the documentation of
        /// <see cref="T:Antlr4.Runtime.IIntStream" />
        /// for a description of
        /// Initializing Methods.</p>
        /// </summary>
        protected internal int p = -1;
        /// <summary>
        /// Indicates whether the
        /// <see cref="F:Antlr4.Runtime.TokenConstants.Eof" />
        /// token has been fetched from
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.tokenSource" />
        /// and added to
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.tokens" />
        /// . This field improves
        /// performance for the following cases:
        /// <ul>
        /// <li>
        /// <see cref="M:Antlr4.Runtime.BufferedTokenStream.Consume" />
        /// : The lookahead check in
        /// <see cref="M:Antlr4.Runtime.BufferedTokenStream.Consume" />
        /// to prevent
        /// consuming the EOF symbol is optimized by checking the values of
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.fetchedEOF" />
        /// and
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.p" />
        /// instead of calling
        /// <see cref="M:Antlr4.Runtime.BufferedTokenStream.La(System.Int32)" />
        /// .</li>
        /// <li>
        /// <see cref="M:Antlr4.Runtime.BufferedTokenStream.Fetch(System.Int32)" />
        /// : The check to prevent adding multiple EOF symbols into
        /// <see cref="F:Antlr4.Runtime.BufferedTokenStream.tokens" />
        /// is trivial with this field.</li>
        /// </ul>
        /// </summary>
        protected internal bool fetchedEOF;

        public MyBufferedTokenStream([NotNull] ITokenSource tokenSource) => this.tokenSource = tokenSource != null ? tokenSource : throw new ArgumentNullException("tokenSource cannot be null");

        public virtual ITokenSource TokenSource => this.tokenSource;

        public virtual int Index => this.p;

        public virtual int Mark() => 0;

        public virtual void Release(int marker) {
        }

        public virtual void Reset() => this.Seek(0);

        public virtual void Seek(int index) {
            this.LazyInit();
            this.p = this.AdjustSeekIndex(index);
        }

        public virtual int Size => this.tokens.Count;

        public virtual void Consume() {
            if ((this.p < 0 || !(!this.fetchedEOF ? this.p < this.tokens.Count : this.p < this.tokens.Count - 1)) && this.La(1) == -1)
                throw new InvalidOperationException("cannot consume EOF");
            if (!this.Sync(this.p + 1))
                return;
            this.p = this.AdjustSeekIndex(this.p + 1);
        }

        /// <summary>
        /// Make sure index
        /// <paramref name="i" />
        /// in tokens has a token.
        /// </summary>
        /// <returns>
        /// 
        /// <see langword="true" />
        /// if a token is located at index
        /// <paramref name="i" />
        /// , otherwise
        /// <see langword="false" />
        /// .
        /// </returns>
        /// <seealso cref="M:Antlr4.Runtime.BufferedTokenStream.Get(System.Int32)" />
        protected internal virtual bool Sync(int i) {
            int n = i - this.tokens.Count + 1;
            return n <= 0 || this.Fetch(n) >= n;
        }

        /// <summary>
        /// Add
        /// <paramref name="n" />
        /// elements to buffer.
        /// </summary>
        /// <returns>The actual number of elements added to the buffer.</returns>
        protected internal virtual int Fetch(int n) {
            if (this.fetchedEOF)
                return 0;
            for (int index = 0; index < n; ++index) {
                IToken token = this.tokenSource.NextToken();
                if (token is IWritableToken)
                    ((IWritableToken)token).TokenIndex = this.tokens.Count;
                this.tokens.Add(token);
                if (token.Type == -1) {
                    this.fetchedEOF = true;
                    return index + 1;
                }
            }
            return n;
        }

        public virtual IToken Get(int i) {
            if (i < 0 || i >= this.tokens.Count)
                throw new ArgumentOutOfRangeException("token index " + (object)i + " out of range 0.." + (object)(this.tokens.Count - 1));
            return this.tokens[i];
        }

        /// <summary>Get all tokens from start..stop inclusively.</summary>
        public virtual IList<IToken> Get(int start, int stop) {
            if (start < 0 || stop < 0)
                return (IList<IToken>)null;
            this.LazyInit();
            IList<IToken> tokenList = (IList<IToken>)new List<IToken>();
            if (stop >= this.tokens.Count)
                stop = this.tokens.Count - 1;
            for (int index = start; index <= stop; ++index) {
                IToken token = this.tokens[index];
                if (token.Type != -1)
                    tokenList.Add(token);
                else
                    break;
            }
            return tokenList;
        }

        public virtual int La(int i) => this.Lt(i).Type;

        protected internal virtual IToken Lb(int k) => this.p - k < 0 ? (IToken)null : this.tokens[this.p - k];

        [return: NotNull]
        public virtual IToken Lt(int k) {
            this.LazyInit();
            if (k == 0)
                return (IToken)null;
            if (k < 0)
                return this.Lb(-k);
            int num = this.p + k - 1;
            this.Sync(num);
            return num >= this.tokens.Count ? this.tokens[this.tokens.Count - 1] : this.tokens[num];
        }

        /// <summary>
        /// Allowed derived classes to modify the behavior of operations which change
        /// the current stream position by adjusting the target token index of a seek
        /// operation.
        /// </summary>
        /// <remarks>
        /// Allowed derived classes to modify the behavior of operations which change
        /// the current stream position by adjusting the target token index of a seek
        /// operation. The default implementation simply returns
        /// <paramref name="i" />
        /// . If an
        /// exception is thrown in this method, the current stream index should not be
        /// changed.
        /// <p>For example,
        /// <see cref="T:Antlr4.Runtime.CommonTokenStream" />
        /// overrides this method to ensure that
        /// the seek target is always an on-channel token.</p>
        /// </remarks>
        /// <param name="i">The target token index.</param>
        /// <returns>The adjusted target token index.</returns>
        protected internal virtual int AdjustSeekIndex(int i) => i;

        protected internal void LazyInit() {
            if (this.p != -1)
                return;
            this.Setup();
        }

        protected internal virtual void Setup() {
            this.Sync(0);
            this.p = this.AdjustSeekIndex(0);
        }

        /// <summary>Reset this token stream by setting its token source.</summary>
        public virtual void SetTokenSource(ITokenSource tokenSource) {
            this.tokenSource = tokenSource;
            this.tokens.Clear();
            this.p = -1;
        }

        public virtual IList<IToken> GetTokens() => this.tokens;

        public virtual IList<IToken> GetTokens(int start, int stop) => this.GetTokens(start, stop, (BitSet)null);

        /// <summary>
        /// Given a start and stop index, return a
        /// <c>List</c>
        /// of all tokens in
        /// the token type
        /// <c>BitSet</c>
        /// .  Return
        /// <see langword="null" />
        /// if no tokens were found.  This
        /// method looks at both on and off channel tokens.
        /// </summary>
        public virtual IList<IToken> GetTokens(int start, int stop, BitSet types) {
            this.LazyInit();
            if (start < 0 || stop >= this.tokens.Count || (stop < 0 || start >= this.tokens.Count))
                throw new ArgumentOutOfRangeException("start " + (object)start + " or stop " + (object)stop + " not in 0.." + (object)(this.tokens.Count - 1));
            if (start > stop)
                return (IList<IToken>)null;
            IList<IToken> tokenList = (IList<IToken>)new List<IToken>();
            for (int index = start; index <= stop; ++index) {
                IToken token = this.tokens[index];
                if (types == null || types.Get(token.Type))
                    tokenList.Add(token);
            }
            if (tokenList.Count == 0)
                tokenList = (IList<IToken>)null;
            return tokenList;
        }

        public virtual IList<IToken> GetTokens(int start, int stop, int ttype) {
            BitSet types = new BitSet(ttype);
            types.Set(ttype);
            return this.GetTokens(start, stop, types);
        }

        /// <summary>Given a starting index, return the index of the next token on channel.</summary>
        /// <remarks>
        /// Given a starting index, return the index of the next token on channel.
        /// Return
        /// <paramref name="i" />
        /// if
        /// <c>tokens[i]</c>
        /// is on channel. Return the index of
        /// the EOF token if there are no tokens on channel between
        /// <paramref name="i" />
        /// and
        /// EOF.
        /// </remarks>
        protected internal virtual int NextTokenOnChannel(int i, int channel) {
            this.Sync(i);
            if (i >= this.Size)
                return this.Size - 1;
            for (IToken token = this.tokens[i]; token.Channel != channel && token.Type != -1; token = this.tokens[i]) {
                ++i;
                this.Sync(i);
            }
            return i;
        }

        /// <summary>
        /// Given a starting index, return the index of the previous token on
        /// channel.
        /// </summary>
        /// <remarks>
        /// Given a starting index, return the index of the previous token on
        /// channel. Return
        /// <paramref name="i" />
        /// if
        /// <c>tokens[i]</c>
        /// is on channel. Return -1
        /// if there are no tokens on channel between
        /// <paramref name="i" />
        /// and 0.
        /// <p>
        /// If
        /// <paramref name="i" />
        /// specifies an index at or after the EOF token, the EOF token
        /// index is returned. This is due to the fact that the EOF token is treated
        /// as though it were on every channel.</p>
        /// </remarks>
        protected internal virtual int PreviousTokenOnChannel(int i, int channel) {
            this.Sync(i);
            if (i >= this.Size)
                return this.Size - 1;
            for (; i >= 0; --i) {
                IToken token = this.tokens[i];
                if (token.Type == -1 || token.Channel == channel)
                    return i;
            }
            return i;
        }

        /// <summary>
        /// Collect all tokens on specified channel to the right of
        /// the current token up until we see a token on
        /// <see cref="F:Antlr4.Runtime.Lexer.DefaultTokenChannel" />
        /// or
        /// EOF. If
        /// <paramref name="channel" />
        /// is
        /// <c>-1</c>
        /// , find any non default channel token.
        /// </summary>
        public virtual IList<IToken> GetHiddenTokensToRight(int tokenIndex, int channel) {
            this.LazyInit();
            if (tokenIndex < 0 || tokenIndex >= this.tokens.Count)
                throw new ArgumentOutOfRangeException(tokenIndex.ToString() + " not in 0.." + (object)(this.tokens.Count - 1));
            int num = this.NextTokenOnChannel(tokenIndex + 1, 0);
            return this.FilterForChannel(tokenIndex + 1, num != -1 ? num : this.Size - 1, channel);
        }

        /// <summary>
        /// Collect all hidden tokens (any off-default channel) to the right of
        /// the current token up until we see a token on
        /// <see cref="F:Antlr4.Runtime.Lexer.DefaultTokenChannel" />
        /// or EOF.
        /// </summary>
        public virtual IList<IToken> GetHiddenTokensToRight(int tokenIndex) => this.GetHiddenTokensToRight(tokenIndex, -1);

        /// <summary>
        /// Collect all tokens on specified channel to the left of
        /// the current token up until we see a token on
        /// <see cref="F:Antlr4.Runtime.Lexer.DefaultTokenChannel" />
        /// .
        /// If
        /// <paramref name="channel" />
        /// is
        /// <c>-1</c>
        /// , find any non default channel token.
        /// </summary>
        public virtual IList<IToken> GetHiddenTokensToLeft(int tokenIndex, int channel) {
            this.LazyInit();
            if (tokenIndex < 0 || tokenIndex >= this.tokens.Count)
                throw new ArgumentOutOfRangeException(tokenIndex.ToString() + " not in 0.." + (object)(this.tokens.Count - 1));
            if (tokenIndex == 0)
                return (IList<IToken>)null;
            int num = this.PreviousTokenOnChannel(tokenIndex - 1, 0);
            return num == tokenIndex - 1 ? (IList<IToken>)null : this.FilterForChannel(num + 1, tokenIndex - 1, channel);
        }

        /// <summary>
        /// Collect all hidden tokens (any off-default channel) to the left of
        /// the current token up until we see a token on
        /// <see cref="F:Antlr4.Runtime.Lexer.DefaultTokenChannel" />
        /// .
        /// </summary>
        public virtual IList<IToken> GetHiddenTokensToLeft(int tokenIndex) => this.GetHiddenTokensToLeft(tokenIndex, -1);

        protected internal virtual IList<IToken> FilterForChannel(
          int from,
          int to,
          int channel) {
            IList<IToken> tokenList = (IList<IToken>)new List<IToken>();
            for (int index = from; index <= to; ++index) {
                IToken token = this.tokens[index];
                if (channel == -1) {
                    if (token.Channel != 0)
                        tokenList.Add(token);
                } else if (token.Channel == channel)
                    tokenList.Add(token);
            }
            return tokenList.Count == 0 ? (IList<IToken>)null : tokenList;
        }

        public virtual string SourceName => this.tokenSource.SourceName;

        /// <summary>Get the text of all tokens in this buffer.</summary>
        [return: NotNull]
        public virtual string GetText() => this.GetText(Interval.Of(0, this.Size - 1));

        [return: NotNull]
        public virtual string GetText(Interval interval) {
            int a = interval.a;
            int num = interval.b;
            if (a < 0 || num < 0)
                return string.Empty;
            this.Fill();
            if (num >= this.tokens.Count)
                num = this.tokens.Count - 1;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = a; index <= num; ++index) {
                IToken token = this.tokens[index];
                if (token.Type != -1)
                    stringBuilder.Append(token.Text);
                else
                    break;
            }
            return stringBuilder.ToString();
        }

        [return: NotNull]
        public virtual string GetText(RuleContext ctx) => this.GetText(ctx.SourceInterval);

        [return: NotNull]
        public virtual string GetText(IToken start, IToken stop) => start != null && stop != null ? this.GetText(Interval.Of(start.TokenIndex, stop.TokenIndex)) : string.Empty;

        /// <summary>Get all tokens from lexer until EOF.</summary>
        public virtual void Fill() {
            this.LazyInit();
            int n = 1000;
            do
                ;
            while (this.Fetch(n) >= n);
        }
    }

    public class MyCommonTokenStream : MyBufferedTokenStream {
        /// <summary>Specifies the channel to use for filtering tokens.</summary>
        /// <remarks>
        /// Specifies the channel to use for filtering tokens.
        /// <p>
        /// The default value is
        /// <see cref="F:Antlr4.Runtime.TokenConstants.DefaultChannel" />
        /// , which matches the
        /// default channel assigned to tokens created by the lexer.</p>
        /// </remarks>
        protected internal int channel;

        /// <summary>
        /// Constructs a new
        /// <see cref="T:Antlr4.Runtime.CommonTokenStream" />
        /// using the specified token
        /// source and the default token channel (
        /// <see cref="F:Antlr4.Runtime.TokenConstants.DefaultChannel" />
        /// ).
        /// </summary>
        /// <param name="tokenSource">The token source.</param>
        public MyCommonTokenStream([NotNull] ITokenSource tokenSource)
          : base(tokenSource) {
        }

        /// <summary>
        /// Constructs a new
        /// <see cref="T:Antlr4.Runtime.CommonTokenStream" />
        /// using the specified token
        /// source and filtering tokens to the specified channel. Only tokens whose
        /// <see cref="P:Antlr4.Runtime.IToken.Channel" />
        /// matches
        /// <paramref name="channel" />
        /// or have the
        /// <see cref="P:Antlr4.Runtime.IToken.Type" />
        /// equal to
        /// <see cref="F:Antlr4.Runtime.TokenConstants.Eof" />
        /// will be returned by the
        /// token stream lookahead methods.
        /// </summary>
        /// <param name="tokenSource">The token source.</param>
        /// <param name="channel">The channel to use for filtering tokens.</param>
        public MyCommonTokenStream([NotNull] ITokenSource tokenSource, int channel)
          : this(tokenSource)
          => this.channel = channel;

        protected internal override int AdjustSeekIndex(int i) => this.NextTokenOnChannel(i, this.channel);

        protected internal override IToken Lb(int k) {
            if (k == 0 || this.p - k < 0)
                return (IToken)null;
            int index1 = this.p;
            for (int index2 = 1; index2 <= k && index1 > 0; ++index2)
                index1 = this.PreviousTokenOnChannel(index1 - 1, this.channel);
            return index1 < 0 ? (IToken)null : this.tokens[index1];
        }

        public override IToken Lt(int k) {
            this.LazyInit();
            if (k == 0)
                return (IToken)null;
            if (k < 0)
                return this.Lb(-k);
            int index1 = this.p;
            for (int index2 = 1; index2 < k; ++index2) {
                if (this.Sync(index1 + 1))
                    index1 = this.NextTokenOnChannel(index1 + 1, this.channel);
            }
            return this.tokens[index1];
        }

        /// <summary>Count EOF just once.</summary>
        public virtual int GetNumberOfOnChannelTokens() {
            int num = 0;
            this.Fill();
            for (int index = 0; index < this.tokens.Count; ++index) {
                IToken token = this.tokens[index];
                if (token.Channel == this.channel)
                    ++num;
                if (token.Type == -1)
                    break;
            }
            return num;
        }
    }
}
