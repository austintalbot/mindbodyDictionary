import Ea, { app as br, BrowserWindow as Sa } from "electron";
import { fileURLToPath as ls } from "node:url";
import et from "node:path";
import Sr from "fs";
import Cr from "path";
import us from "https";
import Ca from "stream";
import hs from "events";
import ds from "buffer";
import Di from "util";
var oe = typeof globalThis < "u" ? globalThis : typeof window < "u" ? window : typeof global < "u" ? global : typeof self < "u" ? self : {}, ne = {}, Ta = {}, Aa = {};
(function(e) {
  Object.defineProperty(e, "__esModule", { value: !0 }), e.changePermissions = e.downloadFile = e.getPath = void 0;
  const r = Ea, t = Sr, i = Cr, a = us, n = () => {
    const f = r.app.getPath("userData");
    return i.resolve(`${f}/extensions`);
  };
  e.getPath = n;
  const o = r.net ? r.net.request : a.get, s = (f, l) => new Promise((w, h) => {
    const d = o(f);
    d.on("response", (g) => {
      if (g.statusCode && g.statusCode >= 300 && g.statusCode < 400 && g.headers.location)
        return (0, e.downloadFile)(g.headers.location, l).then(w).catch(h);
      g.pipe(t.createWriteStream(l)).on("close", w), g.on("error", h);
    }), d.on("error", h), d.end();
  });
  e.downloadFile = s;
  const v = (f, l) => {
    t.readdirSync(f).forEach((h) => {
      const d = i.join(f, h);
      t.chmodSync(d, parseInt(`${l}`, 8)), t.statSync(d).isDirectory() && (0, e.changePermissions)(d, l);
    });
  };
  e.changePermissions = v;
})(Aa);
var xt = {}, Ur = {}, ie = {}, er = { exports: {} }, tr = { exports: {} }, Xi;
function Tr() {
  if (Xi) return tr.exports;
  Xi = 1, typeof process > "u" || !process.version || process.version.indexOf("v0.") === 0 || process.version.indexOf("v1.") === 0 && process.version.indexOf("v1.8.") !== 0 ? tr.exports = { nextTick: e } : tr.exports = process;
  function e(r, t, i, a) {
    if (typeof r != "function")
      throw new TypeError('"callback" argument must be a function');
    var n = arguments.length, o, s;
    switch (n) {
      case 0:
      case 1:
        return process.nextTick(r);
      case 2:
        return process.nextTick(function() {
          r.call(null, t);
        });
      case 3:
        return process.nextTick(function() {
          r.call(null, t, i);
        });
      case 4:
        return process.nextTick(function() {
          r.call(null, t, i, a);
        });
      default:
        for (o = new Array(n - 1), s = 0; s < o.length; )
          o[s++] = arguments[s];
        return process.nextTick(function() {
          r.apply(null, o);
        });
    }
  }
  return tr.exports;
}
var Mr, Ji;
function cs() {
  if (Ji) return Mr;
  Ji = 1;
  var e = {}.toString;
  return Mr = Array.isArray || function(r) {
    return e.call(r) == "[object Array]";
  }, Mr;
}
var $r, Qi;
function Ra() {
  return Qi || (Qi = 1, $r = Ca), $r;
}
var rr = { exports: {} }, en;
function Ar() {
  return en || (en = 1, function(e, r) {
    var t = ds, i = t.Buffer;
    function a(o, s) {
      for (var v in o)
        s[v] = o[v];
    }
    i.from && i.alloc && i.allocUnsafe && i.allocUnsafeSlow ? e.exports = t : (a(t, r), r.Buffer = n);
    function n(o, s, v) {
      return i(o, s, v);
    }
    a(i, n), n.from = function(o, s, v) {
      if (typeof o == "number")
        throw new TypeError("Argument must not be a number");
      return i(o, s, v);
    }, n.alloc = function(o, s, v) {
      if (typeof o != "number")
        throw new TypeError("Argument must be a number");
      var f = i(o);
      return s !== void 0 ? typeof v == "string" ? f.fill(s, v) : f.fill(s) : f.fill(0), f;
    }, n.allocUnsafe = function(o) {
      if (typeof o != "number")
        throw new TypeError("Argument must be a number");
      return i(o);
    }, n.allocUnsafeSlow = function(o) {
      if (typeof o != "number")
        throw new TypeError("Argument must be a number");
      return t.SlowBuffer(o);
    };
  }(rr, rr.exports)), rr.exports;
}
var fe = {}, tn;
function qt() {
  if (tn) return fe;
  tn = 1;
  function e(m) {
    return Array.isArray ? Array.isArray(m) : g(m) === "[object Array]";
  }
  fe.isArray = e;
  function r(m) {
    return typeof m == "boolean";
  }
  fe.isBoolean = r;
  function t(m) {
    return m === null;
  }
  fe.isNull = t;
  function i(m) {
    return m == null;
  }
  fe.isNullOrUndefined = i;
  function a(m) {
    return typeof m == "number";
  }
  fe.isNumber = a;
  function n(m) {
    return typeof m == "string";
  }
  fe.isString = n;
  function o(m) {
    return typeof m == "symbol";
  }
  fe.isSymbol = o;
  function s(m) {
    return m === void 0;
  }
  fe.isUndefined = s;
  function v(m) {
    return g(m) === "[object RegExp]";
  }
  fe.isRegExp = v;
  function f(m) {
    return typeof m == "object" && m !== null;
  }
  fe.isObject = f;
  function l(m) {
    return g(m) === "[object Date]";
  }
  fe.isDate = l;
  function w(m) {
    return g(m) === "[object Error]" || m instanceof Error;
  }
  fe.isError = w;
  function h(m) {
    return typeof m == "function";
  }
  fe.isFunction = h;
  function d(m) {
    return m === null || typeof m == "boolean" || typeof m == "number" || typeof m == "string" || typeof m == "symbol" || // ES6 symbol
    typeof m > "u";
  }
  fe.isPrimitive = d, fe.isBuffer = Buffer.isBuffer;
  function g(m) {
    return Object.prototype.toString.call(m);
  }
  return fe;
}
var ir = { exports: {} }, nr = { exports: {} }, rn;
function vs() {
  return rn || (rn = 1, typeof Object.create == "function" ? nr.exports = function(r, t) {
    t && (r.super_ = t, r.prototype = Object.create(t.prototype, {
      constructor: {
        value: r,
        enumerable: !1,
        writable: !0,
        configurable: !0
      }
    }));
  } : nr.exports = function(r, t) {
    if (t) {
      r.super_ = t;
      var i = function() {
      };
      i.prototype = t.prototype, r.prototype = new i(), r.prototype.constructor = r;
    }
  }), nr.exports;
}
var nn;
function Yt() {
  if (nn) return ir.exports;
  nn = 1;
  try {
    var e = require("util");
    if (typeof e.inherits != "function") throw "";
    ir.exports = e.inherits;
  } catch {
    ir.exports = vs();
  }
  return ir.exports;
}
var Zr = { exports: {} }, an;
function ps() {
  return an || (an = 1, function(e) {
    function r(n, o) {
      if (!(n instanceof o))
        throw new TypeError("Cannot call a class as a function");
    }
    var t = Ar().Buffer, i = Di;
    function a(n, o, s) {
      n.copy(o, s);
    }
    e.exports = function() {
      function n() {
        r(this, n), this.head = null, this.tail = null, this.length = 0;
      }
      return n.prototype.push = function(s) {
        var v = { data: s, next: null };
        this.length > 0 ? this.tail.next = v : this.head = v, this.tail = v, ++this.length;
      }, n.prototype.unshift = function(s) {
        var v = { data: s, next: this.head };
        this.length === 0 && (this.tail = v), this.head = v, ++this.length;
      }, n.prototype.shift = function() {
        if (this.length !== 0) {
          var s = this.head.data;
          return this.length === 1 ? this.head = this.tail = null : this.head = this.head.next, --this.length, s;
        }
      }, n.prototype.clear = function() {
        this.head = this.tail = null, this.length = 0;
      }, n.prototype.join = function(s) {
        if (this.length === 0) return "";
        for (var v = this.head, f = "" + v.data; v = v.next; )
          f += s + v.data;
        return f;
      }, n.prototype.concat = function(s) {
        if (this.length === 0) return t.alloc(0);
        for (var v = t.allocUnsafe(s >>> 0), f = this.head, l = 0; f; )
          a(f.data, v, l), l += f.data.length, f = f.next;
        return v;
      }, n;
    }(), i && i.inspect && i.inspect.custom && (e.exports.prototype[i.inspect.custom] = function() {
      var n = i.inspect({ length: this.length });
      return this.constructor.name + " " + n;
    });
  }(Zr)), Zr.exports;
}
var Wr, on;
function Oa() {
  if (on) return Wr;
  on = 1;
  var e = Tr();
  function r(a, n) {
    var o = this, s = this._readableState && this._readableState.destroyed, v = this._writableState && this._writableState.destroyed;
    return s || v ? (n ? n(a) : a && (this._writableState ? this._writableState.errorEmitted || (this._writableState.errorEmitted = !0, e.nextTick(i, this, a)) : e.nextTick(i, this, a)), this) : (this._readableState && (this._readableState.destroyed = !0), this._writableState && (this._writableState.destroyed = !0), this._destroy(a || null, function(f) {
      !n && f ? o._writableState ? o._writableState.errorEmitted || (o._writableState.errorEmitted = !0, e.nextTick(i, o, f)) : e.nextTick(i, o, f) : n && n(f);
    }), this);
  }
  function t() {
    this._readableState && (this._readableState.destroyed = !1, this._readableState.reading = !1, this._readableState.ended = !1, this._readableState.endEmitted = !1), this._writableState && (this._writableState.destroyed = !1, this._writableState.ended = !1, this._writableState.ending = !1, this._writableState.finalCalled = !1, this._writableState.prefinished = !1, this._writableState.finished = !1, this._writableState.errorEmitted = !1);
  }
  function i(a, n) {
    a.emit("error", n);
  }
  return Wr = {
    destroy: r,
    undestroy: t
  }, Wr;
}
var Hr, sn;
function _s() {
  return sn || (sn = 1, Hr = Di.deprecate), Hr;
}
var qr, fn;
function Da() {
  if (fn) return qr;
  fn = 1;
  var e = Tr();
  qr = m;
  function r(x) {
    var b = this;
    this.next = null, this.entry = null, this.finish = function() {
      Pe(b, x);
    };
  }
  var t = !process.browser && ["v0.10", "v0.9."].indexOf(process.version.slice(0, 5)) > -1 ? setImmediate : e.nextTick, i;
  m.WritableState = d;
  var a = Object.create(qt());
  a.inherits = Yt();
  var n = {
    deprecate: _s()
  }, o = Ra(), s = Ar().Buffer, v = (typeof oe < "u" ? oe : typeof window < "u" ? window : typeof self < "u" ? self : {}).Uint8Array || function() {
  };
  function f(x) {
    return s.from(x);
  }
  function l(x) {
    return s.isBuffer(x) || x instanceof v;
  }
  var w = Oa();
  a.inherits(m, o);
  function h() {
  }
  function d(x, b) {
    i = i || bt(), x = x || {};
    var R = b instanceof i;
    this.objectMode = !!x.objectMode, R && (this.objectMode = this.objectMode || !!x.writableObjectMode);
    var j = x.highWaterMark, Z = x.writableHighWaterMark, Y = this.objectMode ? 16 : 16 * 1024;
    j || j === 0 ? this.highWaterMark = j : R && (Z || Z === 0) ? this.highWaterMark = Z : this.highWaterMark = Y, this.highWaterMark = Math.floor(this.highWaterMark), this.finalCalled = !1, this.needDrain = !1, this.ending = !1, this.ended = !1, this.finished = !1, this.destroyed = !1;
    var te = x.decodeStrings === !1;
    this.decodeStrings = !te, this.defaultEncoding = x.defaultEncoding || "utf8", this.length = 0, this.writing = !1, this.corked = 0, this.sync = !0, this.bufferProcessing = !1, this.onwrite = function(de) {
      L(b, de);
    }, this.writecb = null, this.writelen = 0, this.bufferedRequest = null, this.lastBufferedRequest = null, this.pendingcb = 0, this.prefinished = !1, this.errorEmitted = !1, this.bufferedRequestCount = 0, this.corkedRequestsFree = new r(this);
  }
  d.prototype.getBuffer = function() {
    for (var b = this.bufferedRequest, R = []; b; )
      R.push(b), b = b.next;
    return R;
  }, function() {
    try {
      Object.defineProperty(d.prototype, "buffer", {
        get: n.deprecate(function() {
          return this.getBuffer();
        }, "_writableState.buffer is deprecated. Use _writableState.getBuffer instead.", "DEP0003")
      });
    } catch {
    }
  }();
  var g;
  typeof Symbol == "function" && Symbol.hasInstance && typeof Function.prototype[Symbol.hasInstance] == "function" ? (g = Function.prototype[Symbol.hasInstance], Object.defineProperty(m, Symbol.hasInstance, {
    value: function(x) {
      return g.call(this, x) ? !0 : this !== m ? !1 : x && x._writableState instanceof d;
    }
  })) : g = function(x) {
    return x instanceof this;
  };
  function m(x) {
    if (i = i || bt(), !g.call(m, this) && !(this instanceof i))
      return new m(x);
    this._writableState = new d(x, this), this.writable = !0, x && (typeof x.write == "function" && (this._write = x.write), typeof x.writev == "function" && (this._writev = x.writev), typeof x.destroy == "function" && (this._destroy = x.destroy), typeof x.final == "function" && (this._final = x.final)), o.call(this);
  }
  m.prototype.pipe = function() {
    this.emit("error", new Error("Cannot pipe, not readable"));
  };
  function C(x, b) {
    var R = new Error("write after end");
    x.emit("error", R), e.nextTick(b, R);
  }
  function u(x, b, R, j) {
    var Z = !0, Y = !1;
    return R === null ? Y = new TypeError("May not write null values to stream") : typeof R != "string" && R !== void 0 && !b.objectMode && (Y = new TypeError("Invalid non-string/buffer chunk")), Y && (x.emit("error", Y), e.nextTick(j, Y), Z = !1), Z;
  }
  m.prototype.write = function(x, b, R) {
    var j = this._writableState, Z = !1, Y = !j.objectMode && l(x);
    return Y && !s.isBuffer(x) && (x = f(x)), typeof b == "function" && (R = b, b = null), Y ? b = "buffer" : b || (b = j.defaultEncoding), typeof R != "function" && (R = h), j.ended ? C(this, R) : (Y || u(this, j, x, R)) && (j.pendingcb++, Z = y(this, j, Y, x, b, R)), Z;
  }, m.prototype.cork = function() {
    var x = this._writableState;
    x.corked++;
  }, m.prototype.uncork = function() {
    var x = this._writableState;
    x.corked && (x.corked--, !x.writing && !x.corked && !x.bufferProcessing && x.bufferedRequest && M(this, x));
  }, m.prototype.setDefaultEncoding = function(b) {
    if (typeof b == "string" && (b = b.toLowerCase()), !(["hex", "utf8", "utf-8", "ascii", "binary", "base64", "ucs2", "ucs-2", "utf16le", "utf-16le", "raw"].indexOf((b + "").toLowerCase()) > -1)) throw new TypeError("Unknown encoding: " + b);
    return this._writableState.defaultEncoding = b, this;
  };
  function _(x, b, R) {
    return !x.objectMode && x.decodeStrings !== !1 && typeof b == "string" && (b = s.from(b, R)), b;
  }
  Object.defineProperty(m.prototype, "writableHighWaterMark", {
    // making it explicit this property is not enumerable
    // because otherwise some prototype manipulation in
    // userland will fail
    enumerable: !1,
    get: function() {
      return this._writableState.highWaterMark;
    }
  });
  function y(x, b, R, j, Z, Y) {
    if (!R) {
      var te = _(b, j, Z);
      j !== te && (R = !0, Z = "buffer", j = te);
    }
    var de = b.objectMode ? 1 : j.length;
    b.length += de;
    var Ce = b.length < b.highWaterMark;
    if (Ce || (b.needDrain = !0), b.writing || b.corked) {
      var ue = b.lastBufferedRequest;
      b.lastBufferedRequest = {
        chunk: j,
        encoding: Z,
        isBuf: R,
        callback: Y,
        next: null
      }, ue ? ue.next = b.lastBufferedRequest : b.bufferedRequest = b.lastBufferedRequest, b.bufferedRequestCount += 1;
    } else
      E(x, b, !1, de, j, Z, Y);
    return Ce;
  }
  function E(x, b, R, j, Z, Y, te) {
    b.writelen = j, b.writecb = te, b.writing = !0, b.sync = !0, R ? x._writev(Z, b.onwrite) : x._write(Z, Y, b.onwrite), b.sync = !1;
  }
  function S(x, b, R, j, Z) {
    --b.pendingcb, R ? (e.nextTick(Z, j), e.nextTick($, x, b), x._writableState.errorEmitted = !0, x.emit("error", j)) : (Z(j), x._writableState.errorEmitted = !0, x.emit("error", j), $(x, b));
  }
  function F(x) {
    x.writing = !1, x.writecb = null, x.length -= x.writelen, x.writelen = 0;
  }
  function L(x, b) {
    var R = x._writableState, j = R.sync, Z = R.writecb;
    if (F(R), b) S(x, R, j, b, Z);
    else {
      var Y = D(R);
      !Y && !R.corked && !R.bufferProcessing && R.bufferedRequest && M(x, R), j ? t(B, x, R, Y, Z) : B(x, R, Y, Z);
    }
  }
  function B(x, b, R, j) {
    R || z(x, b), b.pendingcb--, j(), $(x, b);
  }
  function z(x, b) {
    b.length === 0 && b.needDrain && (b.needDrain = !1, x.emit("drain"));
  }
  function M(x, b) {
    b.bufferProcessing = !0;
    var R = b.bufferedRequest;
    if (x._writev && R && R.next) {
      var j = b.bufferedRequestCount, Z = new Array(j), Y = b.corkedRequestsFree;
      Y.entry = R;
      for (var te = 0, de = !0; R; )
        Z[te] = R, R.isBuf || (de = !1), R = R.next, te += 1;
      Z.allBuffers = de, E(x, b, !0, b.length, Z, "", Y.finish), b.pendingcb++, b.lastBufferedRequest = null, Y.next ? (b.corkedRequestsFree = Y.next, Y.next = null) : b.corkedRequestsFree = new r(b), b.bufferedRequestCount = 0;
    } else {
      for (; R; ) {
        var Ce = R.chunk, ue = R.encoding, c = R.callback, p = b.objectMode ? 1 : Ce.length;
        if (E(x, b, !1, p, Ce, ue, c), R = R.next, b.bufferedRequestCount--, b.writing)
          break;
      }
      R === null && (b.lastBufferedRequest = null);
    }
    b.bufferedRequest = R, b.bufferProcessing = !1;
  }
  m.prototype._write = function(x, b, R) {
    R(new Error("_write() is not implemented"));
  }, m.prototype._writev = null, m.prototype.end = function(x, b, R) {
    var j = this._writableState;
    typeof x == "function" ? (R = x, x = null, b = null) : typeof b == "function" && (R = b, b = null), x != null && this.write(x, b), j.corked && (j.corked = 1, this.uncork()), j.ending || Be(this, j, R);
  };
  function D(x) {
    return x.ending && x.length === 0 && x.bufferedRequest === null && !x.finished && !x.writing;
  }
  function X(x, b) {
    x._final(function(R) {
      b.pendingcb--, R && x.emit("error", R), b.prefinished = !0, x.emit("prefinish"), $(x, b);
    });
  }
  function se(x, b) {
    !b.prefinished && !b.finalCalled && (typeof x._final == "function" ? (b.pendingcb++, b.finalCalled = !0, e.nextTick(X, x, b)) : (b.prefinished = !0, x.emit("prefinish")));
  }
  function $(x, b) {
    var R = D(b);
    return R && (se(x, b), b.pendingcb === 0 && (b.finished = !0, x.emit("finish"))), R;
  }
  function Be(x, b, R) {
    b.ending = !0, $(x, b), R && (b.finished ? e.nextTick(R) : x.once("finish", R)), b.ended = !0, x.writable = !1;
  }
  function Pe(x, b, R) {
    var j = x.entry;
    for (x.entry = null; j; ) {
      var Z = j.callback;
      b.pendingcb--, Z(R), j = j.next;
    }
    b.corkedRequestsFree.next = x;
  }
  return Object.defineProperty(m.prototype, "destroyed", {
    get: function() {
      return this._writableState === void 0 ? !1 : this._writableState.destroyed;
    },
    set: function(x) {
      this._writableState && (this._writableState.destroyed = x);
    }
  }), m.prototype.destroy = w.destroy, m.prototype._undestroy = w.undestroy, m.prototype._destroy = function(x, b) {
    this.end(), b(x);
  }, qr;
}
var Yr, ln;
function bt() {
  if (ln) return Yr;
  ln = 1;
  var e = Tr(), r = Object.keys || function(w) {
    var h = [];
    for (var d in w)
      h.push(d);
    return h;
  };
  Yr = v;
  var t = Object.create(qt());
  t.inherits = Yt();
  var i = Ia(), a = Da();
  t.inherits(v, i);
  for (var n = r(a.prototype), o = 0; o < n.length; o++) {
    var s = n[o];
    v.prototype[s] || (v.prototype[s] = a.prototype[s]);
  }
  function v(w) {
    if (!(this instanceof v)) return new v(w);
    i.call(this, w), a.call(this, w), w && w.readable === !1 && (this.readable = !1), w && w.writable === !1 && (this.writable = !1), this.allowHalfOpen = !0, w && w.allowHalfOpen === !1 && (this.allowHalfOpen = !1), this.once("end", f);
  }
  Object.defineProperty(v.prototype, "writableHighWaterMark", {
    // making it explicit this property is not enumerable
    // because otherwise some prototype manipulation in
    // userland will fail
    enumerable: !1,
    get: function() {
      return this._writableState.highWaterMark;
    }
  });
  function f() {
    this.allowHalfOpen || this._writableState.ended || e.nextTick(l, this);
  }
  function l(w) {
    w.end();
  }
  return Object.defineProperty(v.prototype, "destroyed", {
    get: function() {
      return this._readableState === void 0 || this._writableState === void 0 ? !1 : this._readableState.destroyed && this._writableState.destroyed;
    },
    set: function(w) {
      this._readableState === void 0 || this._writableState === void 0 || (this._readableState.destroyed = w, this._writableState.destroyed = w);
    }
  }), v.prototype._destroy = function(w, h) {
    this.push(null), this.end(), e.nextTick(h, w);
  }, Yr;
}
var Kr = {}, un;
function hn() {
  if (un) return Kr;
  un = 1;
  var e = Ar().Buffer, r = e.isEncoding || function(u) {
    switch (u = "" + u, u && u.toLowerCase()) {
      case "hex":
      case "utf8":
      case "utf-8":
      case "ascii":
      case "binary":
      case "base64":
      case "ucs2":
      case "ucs-2":
      case "utf16le":
      case "utf-16le":
      case "raw":
        return !0;
      default:
        return !1;
    }
  };
  function t(u) {
    if (!u) return "utf8";
    for (var _; ; )
      switch (u) {
        case "utf8":
        case "utf-8":
          return "utf8";
        case "ucs2":
        case "ucs-2":
        case "utf16le":
        case "utf-16le":
          return "utf16le";
        case "latin1":
        case "binary":
          return "latin1";
        case "base64":
        case "ascii":
        case "hex":
          return u;
        default:
          if (_) return;
          u = ("" + u).toLowerCase(), _ = !0;
      }
  }
  function i(u) {
    var _ = t(u);
    if (typeof _ != "string" && (e.isEncoding === r || !r(u))) throw new Error("Unknown encoding: " + u);
    return _ || u;
  }
  Kr.StringDecoder = a;
  function a(u) {
    this.encoding = i(u);
    var _;
    switch (this.encoding) {
      case "utf16le":
        this.text = w, this.end = h, _ = 4;
        break;
      case "utf8":
        this.fillLast = v, _ = 4;
        break;
      case "base64":
        this.text = d, this.end = g, _ = 3;
        break;
      default:
        this.write = m, this.end = C;
        return;
    }
    this.lastNeed = 0, this.lastTotal = 0, this.lastChar = e.allocUnsafe(_);
  }
  a.prototype.write = function(u) {
    if (u.length === 0) return "";
    var _, y;
    if (this.lastNeed) {
      if (_ = this.fillLast(u), _ === void 0) return "";
      y = this.lastNeed, this.lastNeed = 0;
    } else
      y = 0;
    return y < u.length ? _ ? _ + this.text(u, y) : this.text(u, y) : _ || "";
  }, a.prototype.end = l, a.prototype.text = f, a.prototype.fillLast = function(u) {
    if (this.lastNeed <= u.length)
      return u.copy(this.lastChar, this.lastTotal - this.lastNeed, 0, this.lastNeed), this.lastChar.toString(this.encoding, 0, this.lastTotal);
    u.copy(this.lastChar, this.lastTotal - this.lastNeed, 0, u.length), this.lastNeed -= u.length;
  };
  function n(u) {
    return u <= 127 ? 0 : u >> 5 === 6 ? 2 : u >> 4 === 14 ? 3 : u >> 3 === 30 ? 4 : u >> 6 === 2 ? -1 : -2;
  }
  function o(u, _, y) {
    var E = _.length - 1;
    if (E < y) return 0;
    var S = n(_[E]);
    return S >= 0 ? (S > 0 && (u.lastNeed = S - 1), S) : --E < y || S === -2 ? 0 : (S = n(_[E]), S >= 0 ? (S > 0 && (u.lastNeed = S - 2), S) : --E < y || S === -2 ? 0 : (S = n(_[E]), S >= 0 ? (S > 0 && (S === 2 ? S = 0 : u.lastNeed = S - 3), S) : 0));
  }
  function s(u, _, y) {
    if ((_[0] & 192) !== 128)
      return u.lastNeed = 0, "�";
    if (u.lastNeed > 1 && _.length > 1) {
      if ((_[1] & 192) !== 128)
        return u.lastNeed = 1, "�";
      if (u.lastNeed > 2 && _.length > 2 && (_[2] & 192) !== 128)
        return u.lastNeed = 2, "�";
    }
  }
  function v(u) {
    var _ = this.lastTotal - this.lastNeed, y = s(this, u);
    if (y !== void 0) return y;
    if (this.lastNeed <= u.length)
      return u.copy(this.lastChar, _, 0, this.lastNeed), this.lastChar.toString(this.encoding, 0, this.lastTotal);
    u.copy(this.lastChar, _, 0, u.length), this.lastNeed -= u.length;
  }
  function f(u, _) {
    var y = o(this, u, _);
    if (!this.lastNeed) return u.toString("utf8", _);
    this.lastTotal = y;
    var E = u.length - (y - this.lastNeed);
    return u.copy(this.lastChar, 0, E), u.toString("utf8", _, E);
  }
  function l(u) {
    var _ = u && u.length ? this.write(u) : "";
    return this.lastNeed ? _ + "�" : _;
  }
  function w(u, _) {
    if ((u.length - _) % 2 === 0) {
      var y = u.toString("utf16le", _);
      if (y) {
        var E = y.charCodeAt(y.length - 1);
        if (E >= 55296 && E <= 56319)
          return this.lastNeed = 2, this.lastTotal = 4, this.lastChar[0] = u[u.length - 2], this.lastChar[1] = u[u.length - 1], y.slice(0, -1);
      }
      return y;
    }
    return this.lastNeed = 1, this.lastTotal = 2, this.lastChar[0] = u[u.length - 1], u.toString("utf16le", _, u.length - 1);
  }
  function h(u) {
    var _ = u && u.length ? this.write(u) : "";
    if (this.lastNeed) {
      var y = this.lastTotal - this.lastNeed;
      return _ + this.lastChar.toString("utf16le", 0, y);
    }
    return _;
  }
  function d(u, _) {
    var y = (u.length - _) % 3;
    return y === 0 ? u.toString("base64", _) : (this.lastNeed = 3 - y, this.lastTotal = 3, y === 1 ? this.lastChar[0] = u[u.length - 1] : (this.lastChar[0] = u[u.length - 2], this.lastChar[1] = u[u.length - 1]), u.toString("base64", _, u.length - y));
  }
  function g(u) {
    var _ = u && u.length ? this.write(u) : "";
    return this.lastNeed ? _ + this.lastChar.toString("base64", 0, 3 - this.lastNeed) : _;
  }
  function m(u) {
    return u.toString(this.encoding);
  }
  function C(u) {
    return u && u.length ? this.write(u) : "";
  }
  return Kr;
}
var Gr, dn;
function Ia() {
  if (dn) return Gr;
  dn = 1;
  var e = Tr();
  Gr = _;
  var r = cs(), t;
  _.ReadableState = u, hs.EventEmitter;
  var i = function(c, p) {
    return c.listeners(p).length;
  }, a = Ra(), n = Ar().Buffer, o = (typeof oe < "u" ? oe : typeof window < "u" ? window : typeof self < "u" ? self : {}).Uint8Array || function() {
  };
  function s(c) {
    return n.from(c);
  }
  function v(c) {
    return n.isBuffer(c) || c instanceof o;
  }
  var f = Object.create(qt());
  f.inherits = Yt();
  var l = Di, w = void 0;
  l && l.debuglog ? w = l.debuglog("stream") : w = function() {
  };
  var h = ps(), d = Oa(), g;
  f.inherits(_, a);
  var m = ["error", "close", "destroy", "pause", "resume"];
  function C(c, p, A) {
    if (typeof c.prependListener == "function") return c.prependListener(p, A);
    !c._events || !c._events[p] ? c.on(p, A) : r(c._events[p]) ? c._events[p].unshift(A) : c._events[p] = [A, c._events[p]];
  }
  function u(c, p) {
    t = t || bt(), c = c || {};
    var A = p instanceof t;
    this.objectMode = !!c.objectMode, A && (this.objectMode = this.objectMode || !!c.readableObjectMode);
    var I = c.highWaterMark, W = c.readableHighWaterMark, P = this.objectMode ? 16 : 16 * 1024;
    I || I === 0 ? this.highWaterMark = I : A && (W || W === 0) ? this.highWaterMark = W : this.highWaterMark = P, this.highWaterMark = Math.floor(this.highWaterMark), this.buffer = new h(), this.length = 0, this.pipes = null, this.pipesCount = 0, this.flowing = null, this.ended = !1, this.endEmitted = !1, this.reading = !1, this.sync = !0, this.needReadable = !1, this.emittedReadable = !1, this.readableListening = !1, this.resumeScheduled = !1, this.destroyed = !1, this.defaultEncoding = c.defaultEncoding || "utf8", this.awaitDrain = 0, this.readingMore = !1, this.decoder = null, this.encoding = null, c.encoding && (g || (g = hn().StringDecoder), this.decoder = new g(c.encoding), this.encoding = c.encoding);
  }
  function _(c) {
    if (t = t || bt(), !(this instanceof _)) return new _(c);
    this._readableState = new u(c, this), this.readable = !0, c && (typeof c.read == "function" && (this._read = c.read), typeof c.destroy == "function" && (this._destroy = c.destroy)), a.call(this);
  }
  Object.defineProperty(_.prototype, "destroyed", {
    get: function() {
      return this._readableState === void 0 ? !1 : this._readableState.destroyed;
    },
    set: function(c) {
      this._readableState && (this._readableState.destroyed = c);
    }
  }), _.prototype.destroy = d.destroy, _.prototype._undestroy = d.undestroy, _.prototype._destroy = function(c, p) {
    this.push(null), p(c);
  }, _.prototype.push = function(c, p) {
    var A = this._readableState, I;
    return A.objectMode ? I = !0 : typeof c == "string" && (p = p || A.defaultEncoding, p !== A.encoding && (c = n.from(c, p), p = ""), I = !0), y(this, c, p, !1, I);
  }, _.prototype.unshift = function(c) {
    return y(this, c, null, !0, !1);
  };
  function y(c, p, A, I, W) {
    var P = c._readableState;
    if (p === null)
      P.reading = !1, M(c, P);
    else {
      var U;
      W || (U = S(P, p)), U ? c.emit("error", U) : P.objectMode || p && p.length > 0 ? (typeof p != "string" && !P.objectMode && Object.getPrototypeOf(p) !== n.prototype && (p = s(p)), I ? P.endEmitted ? c.emit("error", new Error("stream.unshift() after end event")) : E(c, P, p, !0) : P.ended ? c.emit("error", new Error("stream.push() after EOF")) : (P.reading = !1, P.decoder && !A ? (p = P.decoder.write(p), P.objectMode || p.length !== 0 ? E(c, P, p, !1) : se(c, P)) : E(c, P, p, !1))) : I || (P.reading = !1);
    }
    return F(P);
  }
  function E(c, p, A, I) {
    p.flowing && p.length === 0 && !p.sync ? (c.emit("data", A), c.read(0)) : (p.length += p.objectMode ? 1 : A.length, I ? p.buffer.unshift(A) : p.buffer.push(A), p.needReadable && D(c)), se(c, p);
  }
  function S(c, p) {
    var A;
    return !v(p) && typeof p != "string" && p !== void 0 && !c.objectMode && (A = new TypeError("Invalid non-string/buffer chunk")), A;
  }
  function F(c) {
    return !c.ended && (c.needReadable || c.length < c.highWaterMark || c.length === 0);
  }
  _.prototype.isPaused = function() {
    return this._readableState.flowing === !1;
  }, _.prototype.setEncoding = function(c) {
    return g || (g = hn().StringDecoder), this._readableState.decoder = new g(c), this._readableState.encoding = c, this;
  };
  var L = 8388608;
  function B(c) {
    return c >= L ? c = L : (c--, c |= c >>> 1, c |= c >>> 2, c |= c >>> 4, c |= c >>> 8, c |= c >>> 16, c++), c;
  }
  function z(c, p) {
    return c <= 0 || p.length === 0 && p.ended ? 0 : p.objectMode ? 1 : c !== c ? p.flowing && p.length ? p.buffer.head.data.length : p.length : (c > p.highWaterMark && (p.highWaterMark = B(c)), c <= p.length ? c : p.ended ? p.length : (p.needReadable = !0, 0));
  }
  _.prototype.read = function(c) {
    w("read", c), c = parseInt(c, 10);
    var p = this._readableState, A = c;
    if (c !== 0 && (p.emittedReadable = !1), c === 0 && p.needReadable && (p.length >= p.highWaterMark || p.ended))
      return w("read: emitReadable", p.length, p.ended), p.length === 0 && p.ended ? de(this) : D(this), null;
    if (c = z(c, p), c === 0 && p.ended)
      return p.length === 0 && de(this), null;
    var I = p.needReadable;
    w("need readable", I), (p.length === 0 || p.length - c < p.highWaterMark) && (I = !0, w("length less than watermark", I)), p.ended || p.reading ? (I = !1, w("reading or ended", I)) : I && (w("do read"), p.reading = !0, p.sync = !0, p.length === 0 && (p.needReadable = !0), this._read(p.highWaterMark), p.sync = !1, p.reading || (c = z(A, p)));
    var W;
    return c > 0 ? W = j(c, p) : W = null, W === null ? (p.needReadable = !0, c = 0) : p.length -= c, p.length === 0 && (p.ended || (p.needReadable = !0), A !== c && p.ended && de(this)), W !== null && this.emit("data", W), W;
  };
  function M(c, p) {
    if (!p.ended) {
      if (p.decoder) {
        var A = p.decoder.end();
        A && A.length && (p.buffer.push(A), p.length += p.objectMode ? 1 : A.length);
      }
      p.ended = !0, D(c);
    }
  }
  function D(c) {
    var p = c._readableState;
    p.needReadable = !1, p.emittedReadable || (w("emitReadable", p.flowing), p.emittedReadable = !0, p.sync ? e.nextTick(X, c) : X(c));
  }
  function X(c) {
    w("emit readable"), c.emit("readable"), R(c);
  }
  function se(c, p) {
    p.readingMore || (p.readingMore = !0, e.nextTick($, c, p));
  }
  function $(c, p) {
    for (var A = p.length; !p.reading && !p.flowing && !p.ended && p.length < p.highWaterMark && (w("maybeReadMore read 0"), c.read(0), A !== p.length); )
      A = p.length;
    p.readingMore = !1;
  }
  _.prototype._read = function(c) {
    this.emit("error", new Error("_read() is not implemented"));
  }, _.prototype.pipe = function(c, p) {
    var A = this, I = this._readableState;
    switch (I.pipesCount) {
      case 0:
        I.pipes = c;
        break;
      case 1:
        I.pipes = [I.pipes, c];
        break;
      default:
        I.pipes.push(c);
        break;
    }
    I.pipesCount += 1, w("pipe count=%d opts=%j", I.pipesCount, p);
    var W = (!p || p.end !== !1) && c !== process.stdout && c !== process.stderr, P = W ? lt : T;
    I.endEmitted ? e.nextTick(P) : A.once("end", P), c.on("unpipe", U);
    function U(O, N) {
      w("onunpipe"), O === A && N && N.hasUnpiped === !1 && (N.hasUnpiped = !0, jr());
    }
    function lt() {
      w("onend"), c.end();
    }
    var ut = Be(A);
    c.on("drain", ut);
    var Ot = !1;
    function jr() {
      w("cleanup"), c.removeListener("close", ht), c.removeListener("finish", k), c.removeListener("drain", ut), c.removeListener("error", Dt), c.removeListener("unpipe", U), A.removeListener("end", lt), A.removeListener("end", T), A.removeListener("data", Ge), Ot = !0, I.awaitDrain && (!c._writableState || c._writableState.needDrain) && ut();
    }
    var re = !1;
    A.on("data", Ge);
    function Ge(O) {
      w("ondata"), re = !1;
      var N = c.write(O);
      N === !1 && !re && ((I.pipesCount === 1 && I.pipes === c || I.pipesCount > 1 && ue(I.pipes, c) !== -1) && !Ot && (w("false write response, pause", I.awaitDrain), I.awaitDrain++, re = !0), A.pause());
    }
    function Dt(O) {
      w("onerror", O), T(), c.removeListener("error", Dt), i(c, "error") === 0 && c.emit("error", O);
    }
    C(c, "error", Dt);
    function ht() {
      c.removeListener("finish", k), T();
    }
    c.once("close", ht);
    function k() {
      w("onfinish"), c.removeListener("close", ht), T();
    }
    c.once("finish", k);
    function T() {
      w("unpipe"), A.unpipe(c);
    }
    return c.emit("pipe", A), I.flowing || (w("pipe resume"), A.resume()), c;
  };
  function Be(c) {
    return function() {
      var p = c._readableState;
      w("pipeOnDrain", p.awaitDrain), p.awaitDrain && p.awaitDrain--, p.awaitDrain === 0 && i(c, "data") && (p.flowing = !0, R(c));
    };
  }
  _.prototype.unpipe = function(c) {
    var p = this._readableState, A = { hasUnpiped: !1 };
    if (p.pipesCount === 0) return this;
    if (p.pipesCount === 1)
      return c && c !== p.pipes ? this : (c || (c = p.pipes), p.pipes = null, p.pipesCount = 0, p.flowing = !1, c && c.emit("unpipe", this, A), this);
    if (!c) {
      var I = p.pipes, W = p.pipesCount;
      p.pipes = null, p.pipesCount = 0, p.flowing = !1;
      for (var P = 0; P < W; P++)
        I[P].emit("unpipe", this, { hasUnpiped: !1 });
      return this;
    }
    var U = ue(p.pipes, c);
    return U === -1 ? this : (p.pipes.splice(U, 1), p.pipesCount -= 1, p.pipesCount === 1 && (p.pipes = p.pipes[0]), c.emit("unpipe", this, A), this);
  }, _.prototype.on = function(c, p) {
    var A = a.prototype.on.call(this, c, p);
    if (c === "data")
      this._readableState.flowing !== !1 && this.resume();
    else if (c === "readable") {
      var I = this._readableState;
      !I.endEmitted && !I.readableListening && (I.readableListening = I.needReadable = !0, I.emittedReadable = !1, I.reading ? I.length && D(this) : e.nextTick(Pe, this));
    }
    return A;
  }, _.prototype.addListener = _.prototype.on;
  function Pe(c) {
    w("readable nexttick read 0"), c.read(0);
  }
  _.prototype.resume = function() {
    var c = this._readableState;
    return c.flowing || (w("resume"), c.flowing = !0, x(this, c)), this;
  };
  function x(c, p) {
    p.resumeScheduled || (p.resumeScheduled = !0, e.nextTick(b, c, p));
  }
  function b(c, p) {
    p.reading || (w("resume read 0"), c.read(0)), p.resumeScheduled = !1, p.awaitDrain = 0, c.emit("resume"), R(c), p.flowing && !p.reading && c.read(0);
  }
  _.prototype.pause = function() {
    return w("call pause flowing=%j", this._readableState.flowing), this._readableState.flowing !== !1 && (w("pause"), this._readableState.flowing = !1, this.emit("pause")), this;
  };
  function R(c) {
    var p = c._readableState;
    for (w("flow", p.flowing); p.flowing && c.read() !== null; )
      ;
  }
  _.prototype.wrap = function(c) {
    var p = this, A = this._readableState, I = !1;
    c.on("end", function() {
      if (w("wrapped end"), A.decoder && !A.ended) {
        var U = A.decoder.end();
        U && U.length && p.push(U);
      }
      p.push(null);
    }), c.on("data", function(U) {
      if (w("wrapped data"), A.decoder && (U = A.decoder.write(U)), !(A.objectMode && U == null) && !(!A.objectMode && (!U || !U.length))) {
        var lt = p.push(U);
        lt || (I = !0, c.pause());
      }
    });
    for (var W in c)
      this[W] === void 0 && typeof c[W] == "function" && (this[W] = /* @__PURE__ */ function(U) {
        return function() {
          return c[U].apply(c, arguments);
        };
      }(W));
    for (var P = 0; P < m.length; P++)
      c.on(m[P], this.emit.bind(this, m[P]));
    return this._read = function(U) {
      w("wrapped _read", U), I && (I = !1, c.resume());
    }, this;
  }, Object.defineProperty(_.prototype, "readableHighWaterMark", {
    // making it explicit this property is not enumerable
    // because otherwise some prototype manipulation in
    // userland will fail
    enumerable: !1,
    get: function() {
      return this._readableState.highWaterMark;
    }
  }), _._fromList = j;
  function j(c, p) {
    if (p.length === 0) return null;
    var A;
    return p.objectMode ? A = p.buffer.shift() : !c || c >= p.length ? (p.decoder ? A = p.buffer.join("") : p.buffer.length === 1 ? A = p.buffer.head.data : A = p.buffer.concat(p.length), p.buffer.clear()) : A = Z(c, p.buffer, p.decoder), A;
  }
  function Z(c, p, A) {
    var I;
    return c < p.head.data.length ? (I = p.head.data.slice(0, c), p.head.data = p.head.data.slice(c)) : c === p.head.data.length ? I = p.shift() : I = A ? Y(c, p) : te(c, p), I;
  }
  function Y(c, p) {
    var A = p.head, I = 1, W = A.data;
    for (c -= W.length; A = A.next; ) {
      var P = A.data, U = c > P.length ? P.length : c;
      if (U === P.length ? W += P : W += P.slice(0, c), c -= U, c === 0) {
        U === P.length ? (++I, A.next ? p.head = A.next : p.head = p.tail = null) : (p.head = A, A.data = P.slice(U));
        break;
      }
      ++I;
    }
    return p.length -= I, W;
  }
  function te(c, p) {
    var A = n.allocUnsafe(c), I = p.head, W = 1;
    for (I.data.copy(A), c -= I.data.length; I = I.next; ) {
      var P = I.data, U = c > P.length ? P.length : c;
      if (P.copy(A, A.length - c, 0, U), c -= U, c === 0) {
        U === P.length ? (++W, I.next ? p.head = I.next : p.head = p.tail = null) : (p.head = I, I.data = P.slice(U));
        break;
      }
      ++W;
    }
    return p.length -= W, A;
  }
  function de(c) {
    var p = c._readableState;
    if (p.length > 0) throw new Error('"endReadable()" called on non-empty stream');
    p.endEmitted || (p.ended = !0, e.nextTick(Ce, p, c));
  }
  function Ce(c, p) {
    !c.endEmitted && c.length === 0 && (c.endEmitted = !0, p.readable = !1, p.emit("end"));
  }
  function ue(c, p) {
    for (var A = 0, I = c.length; A < I; A++)
      if (c[A] === p) return A;
    return -1;
  }
  return Gr;
}
var Vr, cn;
function Ba() {
  if (cn) return Vr;
  cn = 1, Vr = i;
  var e = bt(), r = Object.create(qt());
  r.inherits = Yt(), r.inherits(i, e);
  function t(o, s) {
    var v = this._transformState;
    v.transforming = !1;
    var f = v.writecb;
    if (!f)
      return this.emit("error", new Error("write callback called multiple times"));
    v.writechunk = null, v.writecb = null, s != null && this.push(s), f(o);
    var l = this._readableState;
    l.reading = !1, (l.needReadable || l.length < l.highWaterMark) && this._read(l.highWaterMark);
  }
  function i(o) {
    if (!(this instanceof i)) return new i(o);
    e.call(this, o), this._transformState = {
      afterTransform: t.bind(this),
      needTransform: !1,
      transforming: !1,
      writecb: null,
      writechunk: null,
      writeencoding: null
    }, this._readableState.needReadable = !0, this._readableState.sync = !1, o && (typeof o.transform == "function" && (this._transform = o.transform), typeof o.flush == "function" && (this._flush = o.flush)), this.on("prefinish", a);
  }
  function a() {
    var o = this;
    typeof this._flush == "function" ? this._flush(function(s, v) {
      n(o, s, v);
    }) : n(this, null, null);
  }
  i.prototype.push = function(o, s) {
    return this._transformState.needTransform = !1, e.prototype.push.call(this, o, s);
  }, i.prototype._transform = function(o, s, v) {
    throw new Error("_transform() is not implemented");
  }, i.prototype._write = function(o, s, v) {
    var f = this._transformState;
    if (f.writecb = v, f.writechunk = o, f.writeencoding = s, !f.transforming) {
      var l = this._readableState;
      (f.needTransform || l.needReadable || l.length < l.highWaterMark) && this._read(l.highWaterMark);
    }
  }, i.prototype._read = function(o) {
    var s = this._transformState;
    s.writechunk !== null && s.writecb && !s.transforming ? (s.transforming = !0, this._transform(s.writechunk, s.writeencoding, s.afterTransform)) : s.needTransform = !0;
  }, i.prototype._destroy = function(o, s) {
    var v = this;
    e.prototype._destroy.call(this, o, function(f) {
      s(f), v.emit("close");
    });
  };
  function n(o, s, v) {
    if (s) return o.emit("error", s);
    if (v != null && o.push(v), o._writableState.length) throw new Error("Calling transform done when ws.length != 0");
    if (o._transformState.transforming) throw new Error("Calling transform done when still transforming");
    return o.push(null);
  }
  return Vr;
}
var Xr, vn;
function gs() {
  if (vn) return Xr;
  vn = 1, Xr = t;
  var e = Ba(), r = Object.create(qt());
  r.inherits = Yt(), r.inherits(t, e);
  function t(i) {
    if (!(this instanceof t)) return new t(i);
    e.call(this, i);
  }
  return t.prototype._transform = function(i, a, n) {
    n(null, i);
  }, Xr;
}
var pn;
function Fa() {
  return pn || (pn = 1, function(e, r) {
    var t = Ca;
    process.env.READABLE_STREAM === "disable" && t ? (e.exports = t, r = e.exports = t.Readable, r.Readable = t.Readable, r.Writable = t.Writable, r.Duplex = t.Duplex, r.Transform = t.Transform, r.PassThrough = t.PassThrough, r.Stream = t) : (r = e.exports = Ia(), r.Stream = t || r, r.Readable = r, r.Writable = Da(), r.Duplex = bt(), r.Transform = Ba(), r.PassThrough = gs());
  }(er, er.exports)), er.exports;
}
var _n, ar;
ie.base64 = !0;
ie.array = !0;
ie.string = !0;
ie.arraybuffer = typeof ArrayBuffer < "u" && typeof Uint8Array < "u";
ie.nodebuffer = typeof Buffer < "u";
ie.uint8array = typeof Uint8Array < "u";
if (typeof ArrayBuffer > "u")
  ar = ie.blob = !1;
else {
  var gn = new ArrayBuffer(0);
  try {
    ar = ie.blob = new Blob([gn], {
      type: "application/zip"
    }).size === 0;
  } catch {
    try {
      var ms = self.BlobBuilder || self.WebKitBlobBuilder || self.MozBlobBuilder || self.MSBlobBuilder, mn = new ms();
      mn.append(gn), ar = ie.blob = mn.getBlob("application/zip").size === 0;
    } catch {
      ar = ie.blob = !1;
    }
  }
}
try {
  _n = ie.nodestream = !!Fa().Readable;
} catch {
  _n = ie.nodestream = !1;
}
var or = {}, wn;
function La() {
  if (wn) return or;
  wn = 1;
  var e = Q(), r = ie, t = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
  return or.encode = function(i) {
    for (var a = [], n, o, s, v, f, l, w, h = 0, d = i.length, g = d, m = e.getTypeOf(i) !== "string"; h < i.length; )
      g = d - h, m ? (n = i[h++], o = h < d ? i[h++] : 0, s = h < d ? i[h++] : 0) : (n = i.charCodeAt(h++), o = h < d ? i.charCodeAt(h++) : 0, s = h < d ? i.charCodeAt(h++) : 0), v = n >> 2, f = (n & 3) << 4 | o >> 4, l = g > 1 ? (o & 15) << 2 | s >> 6 : 64, w = g > 2 ? s & 63 : 64, a.push(t.charAt(v) + t.charAt(f) + t.charAt(l) + t.charAt(w));
    return a.join("");
  }, or.decode = function(i) {
    var a, n, o, s, v, f, l, w = 0, h = 0, d = "data:";
    if (i.substr(0, d.length) === d)
      throw new Error("Invalid base64 input, it looks like a data url.");
    i = i.replace(/[^A-Za-z0-9+/=]/g, "");
    var g = i.length * 3 / 4;
    if (i.charAt(i.length - 1) === t.charAt(64) && g--, i.charAt(i.length - 2) === t.charAt(64) && g--, g % 1 !== 0)
      throw new Error("Invalid base64 input, bad content length.");
    var m;
    for (r.uint8array ? m = new Uint8Array(g | 0) : m = new Array(g | 0); w < i.length; )
      s = t.indexOf(i.charAt(w++)), v = t.indexOf(i.charAt(w++)), f = t.indexOf(i.charAt(w++)), l = t.indexOf(i.charAt(w++)), a = s << 2 | v >> 4, n = (v & 15) << 4 | f >> 2, o = (f & 3) << 6 | l, m[h++] = a, f !== 64 && (m[h++] = n), l !== 64 && (m[h++] = o);
    return m;
  }, or;
}
var Rr = {
  /**
   * True if this is running in Nodejs, will be undefined in a browser.
   * In a browser, browserify won't include this file and the whole module
   * will be resolved an empty object.
   */
  isNode: typeof Buffer < "u",
  /**
   * Create a new nodejs Buffer from an existing content.
   * @param {Object} data the data to pass to the constructor.
   * @param {String} encoding the encoding to use.
   * @return {Buffer} a new Buffer.
   */
  newBufferFrom: function(e, r) {
    if (Buffer.from && Buffer.from !== Uint8Array.from)
      return Buffer.from(e, r);
    if (typeof e == "number")
      throw new Error('The "data" argument must not be a number');
    return new Buffer(e, r);
  },
  /**
   * Create a new nodejs Buffer with the specified size.
   * @param {Integer} size the size of the buffer.
   * @return {Buffer} a new Buffer.
   */
  allocBuffer: function(e) {
    if (Buffer.alloc)
      return Buffer.alloc(e);
    var r = new Buffer(e);
    return r.fill(0), r;
  },
  /**
   * Find out if an object is a Buffer.
   * @param {Object} b the object to test.
   * @return {Boolean} true if the object is a Buffer, false otherwise.
   */
  isBuffer: function(e) {
    return Buffer.isBuffer(e);
  },
  isStream: function(e) {
    return e && typeof e.on == "function" && typeof e.pause == "function" && typeof e.resume == "function";
  }
}, Jr, yn;
function ws() {
  if (yn) return Jr;
  yn = 1;
  var e = oe.MutationObserver || oe.WebKitMutationObserver, r;
  if (process.browser)
    if (e) {
      var t = 0, i = new e(v), a = oe.document.createTextNode("");
      i.observe(a, {
        characterData: !0
      }), r = function() {
        a.data = t = ++t % 2;
      };
    } else if (!oe.setImmediate && typeof oe.MessageChannel < "u") {
      var n = new oe.MessageChannel();
      n.port1.onmessage = v, r = function() {
        n.port2.postMessage(0);
      };
    } else "document" in oe && "onreadystatechange" in oe.document.createElement("script") ? r = function() {
      var l = oe.document.createElement("script");
      l.onreadystatechange = function() {
        v(), l.onreadystatechange = null, l.parentNode.removeChild(l), l = null;
      }, oe.document.documentElement.appendChild(l);
    } : r = function() {
      setTimeout(v, 0);
    };
  else
    r = function() {
      process.nextTick(v);
    };
  var o, s = [];
  function v() {
    o = !0;
    for (var l, w, h = s.length; h; ) {
      for (w = s, s = [], l = -1; ++l < h; )
        w[l]();
      h = s.length;
    }
    o = !1;
  }
  Jr = f;
  function f(l) {
    s.push(l) === 1 && !o && r();
  }
  return Jr;
}
var Qr, bn;
function ys() {
  if (bn) return Qr;
  bn = 1;
  var e = ws();
  function r() {
  }
  var t = {}, i = ["REJECTED"], a = ["FULFILLED"], n = ["PENDING"];
  if (!process.browser)
    var o = ["UNHANDLED"];
  Qr = s;
  function s(u) {
    if (typeof u != "function")
      throw new TypeError("resolver must be a function");
    this.state = n, this.queue = [], this.outcome = void 0, process.browser || (this.handled = o), u !== r && w(this, u);
  }
  s.prototype.finally = function(u) {
    if (typeof u != "function")
      return this;
    var _ = this.constructor;
    return this.then(y, E);
    function y(S) {
      function F() {
        return S;
      }
      return _.resolve(u()).then(F);
    }
    function E(S) {
      function F() {
        throw S;
      }
      return _.resolve(u()).then(F);
    }
  }, s.prototype.catch = function(u) {
    return this.then(null, u);
  }, s.prototype.then = function(u, _) {
    if (typeof u != "function" && this.state === a || typeof _ != "function" && this.state === i)
      return this;
    var y = new this.constructor(r);
    if (process.browser || this.handled === o && (this.handled = null), this.state !== n) {
      var E = this.state === a ? u : _;
      f(y, E, this.outcome);
    } else
      this.queue.push(new v(y, u, _));
    return y;
  };
  function v(u, _, y) {
    this.promise = u, typeof _ == "function" && (this.onFulfilled = _, this.callFulfilled = this.otherCallFulfilled), typeof y == "function" && (this.onRejected = y, this.callRejected = this.otherCallRejected);
  }
  v.prototype.callFulfilled = function(u) {
    t.resolve(this.promise, u);
  }, v.prototype.otherCallFulfilled = function(u) {
    f(this.promise, this.onFulfilled, u);
  }, v.prototype.callRejected = function(u) {
    t.reject(this.promise, u);
  }, v.prototype.otherCallRejected = function(u) {
    f(this.promise, this.onRejected, u);
  };
  function f(u, _, y) {
    e(function() {
      var E;
      try {
        E = _(y);
      } catch (S) {
        return t.reject(u, S);
      }
      E === u ? t.reject(u, new TypeError("Cannot resolve promise with itself")) : t.resolve(u, E);
    });
  }
  t.resolve = function(u, _) {
    var y = h(l, _);
    if (y.status === "error")
      return t.reject(u, y.value);
    var E = y.value;
    if (E)
      w(u, E);
    else {
      u.state = a, u.outcome = _;
      for (var S = -1, F = u.queue.length; ++S < F; )
        u.queue[S].callFulfilled(_);
    }
    return u;
  }, t.reject = function(u, _) {
    u.state = i, u.outcome = _, process.browser || u.handled === o && e(function() {
      u.handled === o && process.emit("unhandledRejection", _, u);
    });
    for (var y = -1, E = u.queue.length; ++y < E; )
      u.queue[y].callRejected(_);
    return u;
  };
  function l(u) {
    var _ = u && u.then;
    if (u && (typeof u == "object" || typeof u == "function") && typeof _ == "function")
      return function() {
        _.apply(u, arguments);
      };
  }
  function w(u, _) {
    var y = !1;
    function E(B) {
      y || (y = !0, t.reject(u, B));
    }
    function S(B) {
      y || (y = !0, t.resolve(u, B));
    }
    function F() {
      _(S, E);
    }
    var L = h(F);
    L.status === "error" && E(L.value);
  }
  function h(u, _) {
    var y = {};
    try {
      y.value = u(_), y.status = "success";
    } catch (E) {
      y.status = "error", y.value = E;
    }
    return y;
  }
  s.resolve = d;
  function d(u) {
    return u instanceof this ? u : t.resolve(new this(r), u);
  }
  s.reject = g;
  function g(u) {
    var _ = new this(r);
    return t.reject(_, u);
  }
  s.all = m;
  function m(u) {
    var _ = this;
    if (Object.prototype.toString.call(u) !== "[object Array]")
      return this.reject(new TypeError("must be an array"));
    var y = u.length, E = !1;
    if (!y)
      return this.resolve([]);
    for (var S = new Array(y), F = 0, L = -1, B = new this(r); ++L < y; )
      z(u[L], L);
    return B;
    function z(M, D) {
      _.resolve(M).then(X, function(se) {
        E || (E = !0, t.reject(B, se));
      });
      function X(se) {
        S[D] = se, ++F === y && !E && (E = !0, t.resolve(B, S));
      }
    }
  }
  s.race = C;
  function C(u) {
    var _ = this;
    if (Object.prototype.toString.call(u) !== "[object Array]")
      return this.reject(new TypeError("must be an array"));
    var y = u.length, E = !1;
    if (!y)
      return this.resolve([]);
    for (var S = -1, F = new this(r); ++S < y; )
      L(u[S]);
    return F;
    function L(B) {
      _.resolve(B).then(function(z) {
        E || (E = !0, t.resolve(F, z));
      }, function(z) {
        E || (E = !0, t.reject(F, z));
      });
    }
  }
  return Qr;
}
var yi = null;
typeof Promise < "u" ? yi = Promise : yi = ys();
var Kt = {
  Promise: yi
};
(function(e, r) {
  if (e.setImmediate)
    return;
  var t = 1, i = {}, a = !1, n = e.document, o;
  function s(_) {
    typeof _ != "function" && (_ = new Function("" + _));
    for (var y = new Array(arguments.length - 1), E = 0; E < y.length; E++)
      y[E] = arguments[E + 1];
    var S = { callback: _, args: y };
    return i[t] = S, o(t), t++;
  }
  function v(_) {
    delete i[_];
  }
  function f(_) {
    var y = _.callback, E = _.args;
    switch (E.length) {
      case 0:
        y();
        break;
      case 1:
        y(E[0]);
        break;
      case 2:
        y(E[0], E[1]);
        break;
      case 3:
        y(E[0], E[1], E[2]);
        break;
      default:
        y.apply(r, E);
        break;
    }
  }
  function l(_) {
    if (a)
      setTimeout(l, 0, _);
    else {
      var y = i[_];
      if (y) {
        a = !0;
        try {
          f(y);
        } finally {
          v(_), a = !1;
        }
      }
    }
  }
  function w() {
    o = function(_) {
      process.nextTick(function() {
        l(_);
      });
    };
  }
  function h() {
    if (e.postMessage && !e.importScripts) {
      var _ = !0, y = e.onmessage;
      return e.onmessage = function() {
        _ = !1;
      }, e.postMessage("", "*"), e.onmessage = y, _;
    }
  }
  function d() {
    var _ = "setImmediate$" + Math.random() + "$", y = function(E) {
      E.source === e && typeof E.data == "string" && E.data.indexOf(_) === 0 && l(+E.data.slice(_.length));
    };
    e.addEventListener ? e.addEventListener("message", y, !1) : e.attachEvent("onmessage", y), o = function(E) {
      e.postMessage(_ + E, "*");
    };
  }
  function g() {
    var _ = new MessageChannel();
    _.port1.onmessage = function(y) {
      var E = y.data;
      l(E);
    }, o = function(y) {
      _.port2.postMessage(y);
    };
  }
  function m() {
    var _ = n.documentElement;
    o = function(y) {
      var E = n.createElement("script");
      E.onreadystatechange = function() {
        l(y), E.onreadystatechange = null, _.removeChild(E), E = null;
      }, _.appendChild(E);
    };
  }
  function C() {
    o = function(_) {
      setTimeout(l, 0, _);
    };
  }
  var u = Object.getPrototypeOf && Object.getPrototypeOf(e);
  u = u && u.setTimeout ? u : e, {}.toString.call(e.process) === "[object process]" ? w() : h() ? d() : e.MessageChannel ? g() : n && "onreadystatechange" in n.createElement("script") ? m() : C(), u.setImmediate = s, u.clearImmediate = v;
})(typeof self > "u" ? oe : self);
var xn;
function Q() {
  return xn || (xn = 1, function(e) {
    var r = ie, t = La(), i = Rr, a = Kt;
    function n(h) {
      var d = null;
      return r.uint8array ? d = new Uint8Array(h.length) : d = new Array(h.length), s(h, d);
    }
    e.newBlob = function(h, d) {
      e.checkSupport("blob");
      try {
        return new Blob([h], {
          type: d
        });
      } catch {
        try {
          var g = self.BlobBuilder || self.WebKitBlobBuilder || self.MozBlobBuilder || self.MSBlobBuilder, m = new g();
          return m.append(h), m.getBlob(d);
        } catch {
          throw new Error("Bug : can't construct the Blob.");
        }
      }
    };
    function o(h) {
      return h;
    }
    function s(h, d) {
      for (var g = 0; g < h.length; ++g)
        d[g] = h.charCodeAt(g) & 255;
      return d;
    }
    var v = {
      /**
       * Transform an array of int into a string, chunk by chunk.
       * See the performances notes on arrayLikeToString.
       * @param {Array|ArrayBuffer|Uint8Array|Buffer} array the array to transform.
       * @param {String} type the type of the array.
       * @param {Integer} chunk the chunk size.
       * @return {String} the resulting string.
       * @throws Error if the chunk is too big for the stack.
       */
      stringifyByChunk: function(h, d, g) {
        var m = [], C = 0, u = h.length;
        if (u <= g)
          return String.fromCharCode.apply(null, h);
        for (; C < u; )
          d === "array" || d === "nodebuffer" ? m.push(String.fromCharCode.apply(null, h.slice(C, Math.min(C + g, u)))) : m.push(String.fromCharCode.apply(null, h.subarray(C, Math.min(C + g, u)))), C += g;
        return m.join("");
      },
      /**
       * Call String.fromCharCode on every item in the array.
       * This is the naive implementation, which generate A LOT of intermediate string.
       * This should be used when everything else fail.
       * @param {Array|ArrayBuffer|Uint8Array|Buffer} array the array to transform.
       * @return {String} the result.
       */
      stringifyByChar: function(h) {
        for (var d = "", g = 0; g < h.length; g++)
          d += String.fromCharCode(h[g]);
        return d;
      },
      applyCanBeUsed: {
        /**
         * true if the browser accepts to use String.fromCharCode on Uint8Array
         */
        uint8array: function() {
          try {
            return r.uint8array && String.fromCharCode.apply(null, new Uint8Array(1)).length === 1;
          } catch {
            return !1;
          }
        }(),
        /**
         * true if the browser accepts to use String.fromCharCode on nodejs Buffer.
         */
        nodebuffer: function() {
          try {
            return r.nodebuffer && String.fromCharCode.apply(null, i.allocBuffer(1)).length === 1;
          } catch {
            return !1;
          }
        }()
      }
    };
    function f(h) {
      var d = 65536, g = e.getTypeOf(h), m = !0;
      if (g === "uint8array" ? m = v.applyCanBeUsed.uint8array : g === "nodebuffer" && (m = v.applyCanBeUsed.nodebuffer), m)
        for (; d > 1; )
          try {
            return v.stringifyByChunk(h, g, d);
          } catch {
            d = Math.floor(d / 2);
          }
      return v.stringifyByChar(h);
    }
    e.applyFromCharCode = f;
    function l(h, d) {
      for (var g = 0; g < h.length; g++)
        d[g] = h[g];
      return d;
    }
    var w = {};
    w.string = {
      string: o,
      array: function(h) {
        return s(h, new Array(h.length));
      },
      arraybuffer: function(h) {
        return w.string.uint8array(h).buffer;
      },
      uint8array: function(h) {
        return s(h, new Uint8Array(h.length));
      },
      nodebuffer: function(h) {
        return s(h, i.allocBuffer(h.length));
      }
    }, w.array = {
      string: f,
      array: o,
      arraybuffer: function(h) {
        return new Uint8Array(h).buffer;
      },
      uint8array: function(h) {
        return new Uint8Array(h);
      },
      nodebuffer: function(h) {
        return i.newBufferFrom(h);
      }
    }, w.arraybuffer = {
      string: function(h) {
        return f(new Uint8Array(h));
      },
      array: function(h) {
        return l(new Uint8Array(h), new Array(h.byteLength));
      },
      arraybuffer: o,
      uint8array: function(h) {
        return new Uint8Array(h);
      },
      nodebuffer: function(h) {
        return i.newBufferFrom(new Uint8Array(h));
      }
    }, w.uint8array = {
      string: f,
      array: function(h) {
        return l(h, new Array(h.length));
      },
      arraybuffer: function(h) {
        return h.buffer;
      },
      uint8array: o,
      nodebuffer: function(h) {
        return i.newBufferFrom(h);
      }
    }, w.nodebuffer = {
      string: f,
      array: function(h) {
        return l(h, new Array(h.length));
      },
      arraybuffer: function(h) {
        return w.nodebuffer.uint8array(h).buffer;
      },
      uint8array: function(h) {
        return l(h, new Uint8Array(h.length));
      },
      nodebuffer: o
    }, e.transformTo = function(h, d) {
      if (d || (d = ""), !h)
        return d;
      e.checkSupport(h);
      var g = e.getTypeOf(d), m = w[g][h](d);
      return m;
    }, e.resolve = function(h) {
      for (var d = h.split("/"), g = [], m = 0; m < d.length; m++) {
        var C = d[m];
        C === "." || C === "" && m !== 0 && m !== d.length - 1 || (C === ".." ? g.pop() : g.push(C));
      }
      return g.join("/");
    }, e.getTypeOf = function(h) {
      if (typeof h == "string")
        return "string";
      if (Object.prototype.toString.call(h) === "[object Array]")
        return "array";
      if (r.nodebuffer && i.isBuffer(h))
        return "nodebuffer";
      if (r.uint8array && h instanceof Uint8Array)
        return "uint8array";
      if (r.arraybuffer && h instanceof ArrayBuffer)
        return "arraybuffer";
    }, e.checkSupport = function(h) {
      var d = r[h.toLowerCase()];
      if (!d)
        throw new Error(h + " is not supported by this platform");
    }, e.MAX_VALUE_16BITS = 65535, e.MAX_VALUE_32BITS = -1, e.pretty = function(h) {
      var d = "", g, m;
      for (m = 0; m < (h || "").length; m++)
        g = h.charCodeAt(m), d += "\\x" + (g < 16 ? "0" : "") + g.toString(16).toUpperCase();
      return d;
    }, e.delay = function(h, d, g) {
      setImmediate(function() {
        h.apply(g || null, d || []);
      });
    }, e.inherits = function(h, d) {
      var g = function() {
      };
      g.prototype = d.prototype, h.prototype = new g();
    }, e.extend = function() {
      var h = {}, d, g;
      for (d = 0; d < arguments.length; d++)
        for (g in arguments[d])
          Object.prototype.hasOwnProperty.call(arguments[d], g) && typeof h[g] > "u" && (h[g] = arguments[d][g]);
      return h;
    }, e.prepareContent = function(h, d, g, m, C) {
      var u = a.Promise.resolve(d).then(function(_) {
        var y = r.blob && (_ instanceof Blob || ["[object File]", "[object Blob]"].indexOf(Object.prototype.toString.call(_)) !== -1);
        return y && typeof FileReader < "u" ? new a.Promise(function(E, S) {
          var F = new FileReader();
          F.onload = function(L) {
            E(L.target.result);
          }, F.onerror = function(L) {
            S(L.target.error);
          }, F.readAsArrayBuffer(_);
        }) : _;
      });
      return u.then(function(_) {
        var y = e.getTypeOf(_);
        return y ? (y === "arraybuffer" ? _ = e.transformTo("uint8array", _) : y === "string" && (C ? _ = t.decode(_) : g && m !== !0 && (_ = n(_))), _) : a.Promise.reject(
          new Error("Can't read the data of '" + h + "'. Is it in a supported JavaScript type (String, Blob, ArrayBuffer, etc) ?")
        );
      });
    };
  }(Ur)), Ur;
}
function Na(e) {
  this.name = e || "default", this.streamInfo = {}, this.generatedError = null, this.extraStreamInfo = {}, this.isPaused = !0, this.isFinished = !1, this.isLocked = !1, this._listeners = {
    data: [],
    end: [],
    error: []
  }, this.previous = null;
}
Na.prototype = {
  /**
   * Push a chunk to the next workers.
   * @param {Object} chunk the chunk to push
   */
  push: function(e) {
    this.emit("data", e);
  },
  /**
   * End the stream.
   * @return {Boolean} true if this call ended the worker, false otherwise.
   */
  end: function() {
    if (this.isFinished)
      return !1;
    this.flush();
    try {
      this.emit("end"), this.cleanUp(), this.isFinished = !0;
    } catch (e) {
      this.emit("error", e);
    }
    return !0;
  },
  /**
   * End the stream with an error.
   * @param {Error} e the error which caused the premature end.
   * @return {Boolean} true if this call ended the worker with an error, false otherwise.
   */
  error: function(e) {
    return this.isFinished ? !1 : (this.isPaused ? this.generatedError = e : (this.isFinished = !0, this.emit("error", e), this.previous && this.previous.error(e), this.cleanUp()), !0);
  },
  /**
   * Add a callback on an event.
   * @param {String} name the name of the event (data, end, error)
   * @param {Function} listener the function to call when the event is triggered
   * @return {GenericWorker} the current object for chainability
   */
  on: function(e, r) {
    return this._listeners[e].push(r), this;
  },
  /**
   * Clean any references when a worker is ending.
   */
  cleanUp: function() {
    this.streamInfo = this.generatedError = this.extraStreamInfo = null, this._listeners = [];
  },
  /**
   * Trigger an event. This will call registered callback with the provided arg.
   * @param {String} name the name of the event (data, end, error)
   * @param {Object} arg the argument to call the callback with.
   */
  emit: function(e, r) {
    if (this._listeners[e])
      for (var t = 0; t < this._listeners[e].length; t++)
        this._listeners[e][t].call(this, r);
  },
  /**
   * Chain a worker with an other.
   * @param {Worker} next the worker receiving events from the current one.
   * @return {worker} the next worker for chainability
   */
  pipe: function(e) {
    return e.registerPrevious(this);
  },
  /**
   * Same as `pipe` in the other direction.
   * Using an API with `pipe(next)` is very easy.
   * Implementing the API with the point of view of the next one registering
   * a source is easier, see the ZipFileWorker.
   * @param {Worker} previous the previous worker, sending events to this one
   * @return {Worker} the current worker for chainability
   */
  registerPrevious: function(e) {
    if (this.isLocked)
      throw new Error("The stream '" + this + "' has already been used.");
    this.streamInfo = e.streamInfo, this.mergeStreamInfo(), this.previous = e;
    var r = this;
    return e.on("data", function(t) {
      r.processChunk(t);
    }), e.on("end", function() {
      r.end();
    }), e.on("error", function(t) {
      r.error(t);
    }), this;
  },
  /**
   * Pause the stream so it doesn't send events anymore.
   * @return {Boolean} true if this call paused the worker, false otherwise.
   */
  pause: function() {
    return this.isPaused || this.isFinished ? !1 : (this.isPaused = !0, this.previous && this.previous.pause(), !0);
  },
  /**
   * Resume a paused stream.
   * @return {Boolean} true if this call resumed the worker, false otherwise.
   */
  resume: function() {
    if (!this.isPaused || this.isFinished)
      return !1;
    this.isPaused = !1;
    var e = !1;
    return this.generatedError && (this.error(this.generatedError), e = !0), this.previous && this.previous.resume(), !e;
  },
  /**
   * Flush any remaining bytes as the stream is ending.
   */
  flush: function() {
  },
  /**
   * Process a chunk. This is usually the method overridden.
   * @param {Object} chunk the chunk to process.
   */
  processChunk: function(e) {
    this.push(e);
  },
  /**
   * Add a key/value to be added in the workers chain streamInfo once activated.
   * @param {String} key the key to use
   * @param {Object} value the associated value
   * @return {Worker} the current worker for chainability
   */
  withStreamInfo: function(e, r) {
    return this.extraStreamInfo[e] = r, this.mergeStreamInfo(), this;
  },
  /**
   * Merge this worker's streamInfo into the chain's streamInfo.
   */
  mergeStreamInfo: function() {
    for (var e in this.extraStreamInfo)
      Object.prototype.hasOwnProperty.call(this.extraStreamInfo, e) && (this.streamInfo[e] = this.extraStreamInfo[e]);
  },
  /**
   * Lock the stream to prevent further updates on the workers chain.
   * After calling this method, all calls to pipe will fail.
   */
  lock: function() {
    if (this.isLocked)
      throw new Error("The stream '" + this + "' has already been used.");
    this.isLocked = !0, this.previous && this.previous.lock();
  },
  /**
   *
   * Pretty print the workers chain.
   */
  toString: function() {
    var e = "Worker " + this.name;
    return this.previous ? this.previous + " -> " + e : e;
  }
};
var ye = Na;
(function(e) {
  for (var r = Q(), t = ie, i = Rr, a = ye, n = new Array(256), o = 0; o < 256; o++)
    n[o] = o >= 252 ? 6 : o >= 248 ? 5 : o >= 240 ? 4 : o >= 224 ? 3 : o >= 192 ? 2 : 1;
  n[254] = n[254] = 1;
  var s = function(h) {
    var d, g, m, C, u, _ = h.length, y = 0;
    for (C = 0; C < _; C++)
      g = h.charCodeAt(C), (g & 64512) === 55296 && C + 1 < _ && (m = h.charCodeAt(C + 1), (m & 64512) === 56320 && (g = 65536 + (g - 55296 << 10) + (m - 56320), C++)), y += g < 128 ? 1 : g < 2048 ? 2 : g < 65536 ? 3 : 4;
    for (t.uint8array ? d = new Uint8Array(y) : d = new Array(y), u = 0, C = 0; u < y; C++)
      g = h.charCodeAt(C), (g & 64512) === 55296 && C + 1 < _ && (m = h.charCodeAt(C + 1), (m & 64512) === 56320 && (g = 65536 + (g - 55296 << 10) + (m - 56320), C++)), g < 128 ? d[u++] = g : g < 2048 ? (d[u++] = 192 | g >>> 6, d[u++] = 128 | g & 63) : g < 65536 ? (d[u++] = 224 | g >>> 12, d[u++] = 128 | g >>> 6 & 63, d[u++] = 128 | g & 63) : (d[u++] = 240 | g >>> 18, d[u++] = 128 | g >>> 12 & 63, d[u++] = 128 | g >>> 6 & 63, d[u++] = 128 | g & 63);
    return d;
  }, v = function(h, d) {
    var g;
    for (d = d || h.length, d > h.length && (d = h.length), g = d - 1; g >= 0 && (h[g] & 192) === 128; )
      g--;
    return g < 0 || g === 0 ? d : g + n[h[g]] > d ? g : d;
  }, f = function(h) {
    var d, g, m, C, u = h.length, _ = new Array(u * 2);
    for (g = 0, d = 0; d < u; ) {
      if (m = h[d++], m < 128) {
        _[g++] = m;
        continue;
      }
      if (C = n[m], C > 4) {
        _[g++] = 65533, d += C - 1;
        continue;
      }
      for (m &= C === 2 ? 31 : C === 3 ? 15 : 7; C > 1 && d < u; )
        m = m << 6 | h[d++] & 63, C--;
      if (C > 1) {
        _[g++] = 65533;
        continue;
      }
      m < 65536 ? _[g++] = m : (m -= 65536, _[g++] = 55296 | m >> 10 & 1023, _[g++] = 56320 | m & 1023);
    }
    return _.length !== g && (_.subarray ? _ = _.subarray(0, g) : _.length = g), r.applyFromCharCode(_);
  };
  e.utf8encode = function(d) {
    return t.nodebuffer ? i.newBufferFrom(d, "utf-8") : s(d);
  }, e.utf8decode = function(d) {
    return t.nodebuffer ? r.transformTo("nodebuffer", d).toString("utf-8") : (d = r.transformTo(t.uint8array ? "uint8array" : "array", d), f(d));
  };
  function l() {
    a.call(this, "utf-8 decode"), this.leftOver = null;
  }
  r.inherits(l, a), l.prototype.processChunk = function(h) {
    var d = r.transformTo(t.uint8array ? "uint8array" : "array", h.data);
    if (this.leftOver && this.leftOver.length) {
      if (t.uint8array) {
        var g = d;
        d = new Uint8Array(g.length + this.leftOver.length), d.set(this.leftOver, 0), d.set(g, this.leftOver.length);
      } else
        d = this.leftOver.concat(d);
      this.leftOver = null;
    }
    var m = v(d), C = d;
    m !== d.length && (t.uint8array ? (C = d.subarray(0, m), this.leftOver = d.subarray(m, d.length)) : (C = d.slice(0, m), this.leftOver = d.slice(m, d.length))), this.push({
      data: e.utf8decode(C),
      meta: h.meta
    });
  }, l.prototype.flush = function() {
    this.leftOver && this.leftOver.length && (this.push({
      data: e.utf8decode(this.leftOver),
      meta: {}
    }), this.leftOver = null);
  }, e.Utf8DecodeWorker = l;
  function w() {
    a.call(this, "utf-8 encode");
  }
  r.inherits(w, a), w.prototype.processChunk = function(h) {
    this.push({
      data: e.utf8encode(h.data),
      meta: h.meta
    });
  }, e.Utf8EncodeWorker = w;
})(xt);
var za = ye, Pa = Q();
function Ii(e) {
  za.call(this, "ConvertWorker to " + e), this.destType = e;
}
Pa.inherits(Ii, za);
Ii.prototype.processChunk = function(e) {
  this.push({
    data: Pa.transformTo(this.destType, e.data),
    meta: e.meta
  });
};
var bs = Ii, ei, kn;
function xs() {
  if (kn) return ei;
  kn = 1;
  var e = Fa().Readable, r = Q();
  r.inherits(t, e);
  function t(i, a, n) {
    e.call(this, a), this._helper = i;
    var o = this;
    i.on("data", function(s, v) {
      o.push(s) || o._helper.pause(), n && n(v);
    }).on("error", function(s) {
      o.emit("error", s);
    }).on("end", function() {
      o.push(null);
    });
  }
  return t.prototype._read = function() {
    this._helper.resume();
  }, ei = t, ei;
}
var Qe = Q(), ks = bs, Es = ye, Ss = La(), Cs = ie, Ts = Kt, ja = null;
if (Cs.nodestream)
  try {
    ja = xs();
  } catch {
  }
function As(e, r, t) {
  switch (e) {
    case "blob":
      return Qe.newBlob(Qe.transformTo("arraybuffer", r), t);
    case "base64":
      return Ss.encode(r);
    default:
      return Qe.transformTo(e, r);
  }
}
function Rs(e, r) {
  var t, i = 0, a = null, n = 0;
  for (t = 0; t < r.length; t++)
    n += r[t].length;
  switch (e) {
    case "string":
      return r.join("");
    case "array":
      return Array.prototype.concat.apply([], r);
    case "uint8array":
      for (a = new Uint8Array(n), t = 0; t < r.length; t++)
        a.set(r[t], i), i += r[t].length;
      return a;
    case "nodebuffer":
      return Buffer.concat(r);
    default:
      throw new Error("concat : unsupported type '" + e + "'");
  }
}
function Os(e, r) {
  return new Ts.Promise(function(t, i) {
    var a = [], n = e._internalType, o = e._outputType, s = e._mimeType;
    e.on("data", function(v, f) {
      a.push(v), r && r(f);
    }).on("error", function(v) {
      a = [], i(v);
    }).on("end", function() {
      try {
        var v = As(o, Rs(n, a), s);
        t(v);
      } catch (f) {
        i(f);
      }
      a = [];
    }).resume();
  });
}
function Ua(e, r, t) {
  var i = r;
  switch (r) {
    case "blob":
    case "arraybuffer":
      i = "uint8array";
      break;
    case "base64":
      i = "string";
      break;
  }
  try {
    this._internalType = i, this._outputType = r, this._mimeType = t, Qe.checkSupport(i), this._worker = e.pipe(new ks(i)), e.lock();
  } catch (a) {
    this._worker = new Es("error"), this._worker.error(a);
  }
}
Ua.prototype = {
  /**
   * Listen a StreamHelper, accumulate its content and concatenate it into a
   * complete block.
   * @param {Function} updateCb the update callback.
   * @return Promise the promise for the accumulation.
   */
  accumulate: function(e) {
    return Os(this, e);
  },
  /**
   * Add a listener on an event triggered on a stream.
   * @param {String} evt the name of the event
   * @param {Function} fn the listener
   * @return {StreamHelper} the current helper.
   */
  on: function(e, r) {
    var t = this;
    return e === "data" ? this._worker.on(e, function(i) {
      r.call(t, i.data, i.meta);
    }) : this._worker.on(e, function() {
      Qe.delay(r, arguments, t);
    }), this;
  },
  /**
   * Resume the flow of chunks.
   * @return {StreamHelper} the current helper.
   */
  resume: function() {
    return Qe.delay(this._worker.resume, [], this._worker), this;
  },
  /**
   * Pause the flow of chunks.
   * @return {StreamHelper} the current helper.
   */
  pause: function() {
    return this._worker.pause(), this;
  },
  /**
   * Return a nodejs stream for this helper.
   * @param {Function} updateCb the update callback.
   * @return {NodejsStreamOutputAdapter} the nodejs stream.
   */
  toNodejsStream: function(e) {
    if (Qe.checkSupport("nodestream"), this._outputType !== "nodebuffer")
      throw new Error(this._outputType + " is not supported by this method");
    return new ja(this, {
      objectMode: this._outputType !== "nodebuffer"
    }, e);
  }
};
var Ma = Ua, be = {};
be.base64 = !1;
be.binary = !1;
be.dir = !1;
be.createFolders = !0;
be.date = null;
be.compression = null;
be.compressionOptions = null;
be.comment = null;
be.unixPermissions = null;
be.dosPermissions = null;
var Or = Q(), Dr = ye, Ds = 16 * 1024;
function kt(e) {
  Dr.call(this, "DataWorker");
  var r = this;
  this.dataIsReady = !1, this.index = 0, this.max = 0, this.data = null, this.type = "", this._tickScheduled = !1, e.then(function(t) {
    r.dataIsReady = !0, r.data = t, r.max = t && t.length || 0, r.type = Or.getTypeOf(t), r.isPaused || r._tickAndRepeat();
  }, function(t) {
    r.error(t);
  });
}
Or.inherits(kt, Dr);
kt.prototype.cleanUp = function() {
  Dr.prototype.cleanUp.call(this), this.data = null;
};
kt.prototype.resume = function() {
  return Dr.prototype.resume.call(this) ? (!this._tickScheduled && this.dataIsReady && (this._tickScheduled = !0, Or.delay(this._tickAndRepeat, [], this)), !0) : !1;
};
kt.prototype._tickAndRepeat = function() {
  this._tickScheduled = !1, !(this.isPaused || this.isFinished) && (this._tick(), this.isFinished || (Or.delay(this._tickAndRepeat, [], this), this._tickScheduled = !0));
};
kt.prototype._tick = function() {
  if (this.isPaused || this.isFinished)
    return !1;
  var e = Ds, r = null, t = Math.min(this.max, this.index + e);
  if (this.index >= this.max)
    return this.end();
  switch (this.type) {
    case "string":
      r = this.data.substring(this.index, t);
      break;
    case "uint8array":
      r = this.data.subarray(this.index, t);
      break;
    case "array":
    case "nodebuffer":
      r = this.data.slice(this.index, t);
      break;
  }
  return this.index = t, this.push({
    data: r,
    meta: {
      percent: this.max ? this.index / this.max * 100 : 0
    }
  });
};
var $a = kt, Is = Q();
function Bs() {
  for (var e, r = [], t = 0; t < 256; t++) {
    e = t;
    for (var i = 0; i < 8; i++)
      e = e & 1 ? 3988292384 ^ e >>> 1 : e >>> 1;
    r[t] = e;
  }
  return r;
}
var Za = Bs();
function Fs(e, r, t, i) {
  var a = Za, n = i + t;
  e = e ^ -1;
  for (var o = i; o < n; o++)
    e = e >>> 8 ^ a[(e ^ r[o]) & 255];
  return e ^ -1;
}
function Ls(e, r, t, i) {
  var a = Za, n = i + t;
  e = e ^ -1;
  for (var o = i; o < n; o++)
    e = e >>> 8 ^ a[(e ^ r.charCodeAt(o)) & 255];
  return e ^ -1;
}
var Bi = function(r, t) {
  if (typeof r > "u" || !r.length)
    return 0;
  var i = Is.getTypeOf(r) !== "string";
  return i ? Fs(t | 0, r, r.length, 0) : Ls(t | 0, r, r.length, 0);
}, Wa = ye, Ns = Bi, zs = Q();
function Fi() {
  Wa.call(this, "Crc32Probe"), this.withStreamInfo("crc32", 0);
}
zs.inherits(Fi, Wa);
Fi.prototype.processChunk = function(e) {
  this.streamInfo.crc32 = Ns(e.data, this.streamInfo.crc32 || 0), this.push(e);
};
var Ha = Fi, Ps = Q(), Li = ye;
function Ni(e) {
  Li.call(this, "DataLengthProbe for " + e), this.propName = e, this.withStreamInfo(e, 0);
}
Ps.inherits(Ni, Li);
Ni.prototype.processChunk = function(e) {
  if (e) {
    var r = this.streamInfo[this.propName] || 0;
    this.streamInfo[this.propName] = r + e.data.length;
  }
  Li.prototype.processChunk.call(this, e);
};
var js = Ni, En = Kt, Sn = $a, Us = Ha, bi = js;
function zi(e, r, t, i, a) {
  this.compressedSize = e, this.uncompressedSize = r, this.crc32 = t, this.compression = i, this.compressedContent = a;
}
zi.prototype = {
  /**
   * Create a worker to get the uncompressed content.
   * @return {GenericWorker} the worker.
   */
  getContentWorker: function() {
    var e = new Sn(En.Promise.resolve(this.compressedContent)).pipe(this.compression.uncompressWorker()).pipe(new bi("data_length")), r = this;
    return e.on("end", function() {
      if (this.streamInfo.data_length !== r.uncompressedSize)
        throw new Error("Bug : uncompressed data size mismatch");
    }), e;
  },
  /**
   * Create a worker to get the compressed content.
   * @return {GenericWorker} the worker.
   */
  getCompressedWorker: function() {
    return new Sn(En.Promise.resolve(this.compressedContent)).withStreamInfo("compressedSize", this.compressedSize).withStreamInfo("uncompressedSize", this.uncompressedSize).withStreamInfo("crc32", this.crc32).withStreamInfo("compression", this.compression);
  }
};
zi.createWorkerFrom = function(e, r, t) {
  return e.pipe(new Us()).pipe(new bi("uncompressedSize")).pipe(r.compressWorker(t)).pipe(new bi("compressedSize")).withStreamInfo("compression", r);
};
var Pi = zi, Ms = Ma, $s = $a, ti = xt, ri = Pi, Cn = ye, ji = function(e, r, t) {
  this.name = e, this.dir = t.dir, this.date = t.date, this.comment = t.comment, this.unixPermissions = t.unixPermissions, this.dosPermissions = t.dosPermissions, this._data = r, this._dataBinary = t.binary, this.options = {
    compression: t.compression,
    compressionOptions: t.compressionOptions
  };
};
ji.prototype = {
  /**
   * Create an internal stream for the content of this object.
   * @param {String} type the type of each chunk.
   * @return StreamHelper the stream.
   */
  internalStream: function(e) {
    var r = null, t = "string";
    try {
      if (!e)
        throw new Error("No output type specified.");
      t = e.toLowerCase();
      var i = t === "string" || t === "text";
      (t === "binarystring" || t === "text") && (t = "string"), r = this._decompressWorker();
      var a = !this._dataBinary;
      a && !i && (r = r.pipe(new ti.Utf8EncodeWorker())), !a && i && (r = r.pipe(new ti.Utf8DecodeWorker()));
    } catch (n) {
      r = new Cn("error"), r.error(n);
    }
    return new Ms(r, t, "");
  },
  /**
   * Prepare the content in the asked type.
   * @param {String} type the type of the result.
   * @param {Function} onUpdate a function to call on each internal update.
   * @return Promise the promise of the result.
   */
  async: function(e, r) {
    return this.internalStream(e).accumulate(r);
  },
  /**
   * Prepare the content as a nodejs stream.
   * @param {String} type the type of each chunk.
   * @param {Function} onUpdate a function to call on each internal update.
   * @return Stream the stream.
   */
  nodeStream: function(e, r) {
    return this.internalStream(e || "nodebuffer").toNodejsStream(r);
  },
  /**
   * Return a worker for the compressed content.
   * @private
   * @param {Object} compression the compression object to use.
   * @param {Object} compressionOptions the options to use when compressing.
   * @return Worker the worker.
   */
  _compressWorker: function(e, r) {
    if (this._data instanceof ri && this._data.compression.magic === e.magic)
      return this._data.getCompressedWorker();
    var t = this._decompressWorker();
    return this._dataBinary || (t = t.pipe(new ti.Utf8EncodeWorker())), ri.createWorkerFrom(t, e, r);
  },
  /**
   * Return a worker for the decompressed content.
   * @private
   * @return Worker the worker.
   */
  _decompressWorker: function() {
    return this._data instanceof ri ? this._data.getContentWorker() : this._data instanceof Cn ? this._data : new $s(this._data);
  }
};
var Tn = ["asText", "asBinary", "asNodeBuffer", "asUint8Array", "asArrayBuffer"], Zs = function() {
  throw new Error("This method has been removed in JSZip 3.0, please check the upgrade guide.");
};
for (var ii = 0; ii < Tn.length; ii++)
  ji.prototype[Tn[ii]] = Zs;
var Ws = ji, qa = {}, Ir = {}, Br = {}, ze = {};
(function(e) {
  var r = typeof Uint8Array < "u" && typeof Uint16Array < "u" && typeof Int32Array < "u";
  function t(n, o) {
    return Object.prototype.hasOwnProperty.call(n, o);
  }
  e.assign = function(n) {
    for (var o = Array.prototype.slice.call(arguments, 1); o.length; ) {
      var s = o.shift();
      if (s) {
        if (typeof s != "object")
          throw new TypeError(s + "must be non-object");
        for (var v in s)
          t(s, v) && (n[v] = s[v]);
      }
    }
    return n;
  }, e.shrinkBuf = function(n, o) {
    return n.length === o ? n : n.subarray ? n.subarray(0, o) : (n.length = o, n);
  };
  var i = {
    arraySet: function(n, o, s, v, f) {
      if (o.subarray && n.subarray) {
        n.set(o.subarray(s, s + v), f);
        return;
      }
      for (var l = 0; l < v; l++)
        n[f + l] = o[s + l];
    },
    // Join array of chunks to single array.
    flattenChunks: function(n) {
      var o, s, v, f, l, w;
      for (v = 0, o = 0, s = n.length; o < s; o++)
        v += n[o].length;
      for (w = new Uint8Array(v), f = 0, o = 0, s = n.length; o < s; o++)
        l = n[o], w.set(l, f), f += l.length;
      return w;
    }
  }, a = {
    arraySet: function(n, o, s, v, f) {
      for (var l = 0; l < v; l++)
        n[f + l] = o[s + l];
    },
    // Join array of chunks to single array.
    flattenChunks: function(n) {
      return [].concat.apply([], n);
    }
  };
  e.setTyped = function(n) {
    n ? (e.Buf8 = Uint8Array, e.Buf16 = Uint16Array, e.Buf32 = Int32Array, e.assign(e, i)) : (e.Buf8 = Array, e.Buf16 = Array, e.Buf32 = Array, e.assign(e, a));
  }, e.setTyped(r);
})(ze);
var Gt = {}, Ie = {}, Et = {}, Hs = ze, qs = 4, An = 0, Rn = 1, Ys = 2;
function St(e) {
  for (var r = e.length; --r >= 0; )
    e[r] = 0;
}
var Ks = 0, Ya = 1, Gs = 2, Vs = 3, Xs = 258, Ui = 29, Vt = 256, Mt = Vt + 1 + Ui, gt = 30, Mi = 19, Ka = 2 * Mt + 1, Xe = 15, ni = 16, Js = 7, $i = 256, Ga = 16, Va = 17, Xa = 18, xi = (
  /* extra bits for each length code */
  [0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0]
), pr = (
  /* extra bits for each distance code */
  [0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13]
), Qs = (
  /* extra bits for each bit length code */
  [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7]
), Ja = [16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15], ef = 512, Ne = new Array((Mt + 2) * 2);
St(Ne);
var Ft = new Array(gt * 2);
St(Ft);
var $t = new Array(ef);
St($t);
var Zt = new Array(Xs - Vs + 1);
St(Zt);
var Zi = new Array(Ui);
St(Zi);
var xr = new Array(gt);
St(xr);
function ai(e, r, t, i, a) {
  this.static_tree = e, this.extra_bits = r, this.extra_base = t, this.elems = i, this.max_length = a, this.has_stree = e && e.length;
}
var Qa, eo, to;
function oi(e, r) {
  this.dyn_tree = e, this.max_code = 0, this.stat_desc = r;
}
function ro(e) {
  return e < 256 ? $t[e] : $t[256 + (e >>> 7)];
}
function Wt(e, r) {
  e.pending_buf[e.pending++] = r & 255, e.pending_buf[e.pending++] = r >>> 8 & 255;
}
function ce(e, r, t) {
  e.bi_valid > ni - t ? (e.bi_buf |= r << e.bi_valid & 65535, Wt(e, e.bi_buf), e.bi_buf = r >> ni - e.bi_valid, e.bi_valid += t - ni) : (e.bi_buf |= r << e.bi_valid & 65535, e.bi_valid += t);
}
function Oe(e, r, t) {
  ce(
    e,
    t[r * 2],
    t[r * 2 + 1]
    /*.Len*/
  );
}
function io(e, r) {
  var t = 0;
  do
    t |= e & 1, e >>>= 1, t <<= 1;
  while (--r > 0);
  return t >>> 1;
}
function tf(e) {
  e.bi_valid === 16 ? (Wt(e, e.bi_buf), e.bi_buf = 0, e.bi_valid = 0) : e.bi_valid >= 8 && (e.pending_buf[e.pending++] = e.bi_buf & 255, e.bi_buf >>= 8, e.bi_valid -= 8);
}
function rf(e, r) {
  var t = r.dyn_tree, i = r.max_code, a = r.stat_desc.static_tree, n = r.stat_desc.has_stree, o = r.stat_desc.extra_bits, s = r.stat_desc.extra_base, v = r.stat_desc.max_length, f, l, w, h, d, g, m = 0;
  for (h = 0; h <= Xe; h++)
    e.bl_count[h] = 0;
  for (t[e.heap[e.heap_max] * 2 + 1] = 0, f = e.heap_max + 1; f < Ka; f++)
    l = e.heap[f], h = t[t[l * 2 + 1] * 2 + 1] + 1, h > v && (h = v, m++), t[l * 2 + 1] = h, !(l > i) && (e.bl_count[h]++, d = 0, l >= s && (d = o[l - s]), g = t[l * 2], e.opt_len += g * (h + d), n && (e.static_len += g * (a[l * 2 + 1] + d)));
  if (m !== 0) {
    do {
      for (h = v - 1; e.bl_count[h] === 0; )
        h--;
      e.bl_count[h]--, e.bl_count[h + 1] += 2, e.bl_count[v]--, m -= 2;
    } while (m > 0);
    for (h = v; h !== 0; h--)
      for (l = e.bl_count[h]; l !== 0; )
        w = e.heap[--f], !(w > i) && (t[w * 2 + 1] !== h && (e.opt_len += (h - t[w * 2 + 1]) * t[w * 2], t[w * 2 + 1] = h), l--);
  }
}
function no(e, r, t) {
  var i = new Array(Xe + 1), a = 0, n, o;
  for (n = 1; n <= Xe; n++)
    i[n] = a = a + t[n - 1] << 1;
  for (o = 0; o <= r; o++) {
    var s = e[o * 2 + 1];
    s !== 0 && (e[o * 2] = io(i[s]++, s));
  }
}
function nf() {
  var e, r, t, i, a, n = new Array(Xe + 1);
  for (t = 0, i = 0; i < Ui - 1; i++)
    for (Zi[i] = t, e = 0; e < 1 << xi[i]; e++)
      Zt[t++] = i;
  for (Zt[t - 1] = i, a = 0, i = 0; i < 16; i++)
    for (xr[i] = a, e = 0; e < 1 << pr[i]; e++)
      $t[a++] = i;
  for (a >>= 7; i < gt; i++)
    for (xr[i] = a << 7, e = 0; e < 1 << pr[i] - 7; e++)
      $t[256 + a++] = i;
  for (r = 0; r <= Xe; r++)
    n[r] = 0;
  for (e = 0; e <= 143; )
    Ne[e * 2 + 1] = 8, e++, n[8]++;
  for (; e <= 255; )
    Ne[e * 2 + 1] = 9, e++, n[9]++;
  for (; e <= 279; )
    Ne[e * 2 + 1] = 7, e++, n[7]++;
  for (; e <= 287; )
    Ne[e * 2 + 1] = 8, e++, n[8]++;
  for (no(Ne, Mt + 1, n), e = 0; e < gt; e++)
    Ft[e * 2 + 1] = 5, Ft[e * 2] = io(e, 5);
  Qa = new ai(Ne, xi, Vt + 1, Mt, Xe), eo = new ai(Ft, pr, 0, gt, Xe), to = new ai(new Array(0), Qs, 0, Mi, Js);
}
function ao(e) {
  var r;
  for (r = 0; r < Mt; r++)
    e.dyn_ltree[r * 2] = 0;
  for (r = 0; r < gt; r++)
    e.dyn_dtree[r * 2] = 0;
  for (r = 0; r < Mi; r++)
    e.bl_tree[r * 2] = 0;
  e.dyn_ltree[$i * 2] = 1, e.opt_len = e.static_len = 0, e.last_lit = e.matches = 0;
}
function oo(e) {
  e.bi_valid > 8 ? Wt(e, e.bi_buf) : e.bi_valid > 0 && (e.pending_buf[e.pending++] = e.bi_buf), e.bi_buf = 0, e.bi_valid = 0;
}
function af(e, r, t, i) {
  oo(e), Wt(e, t), Wt(e, ~t), Hs.arraySet(e.pending_buf, e.window, r, t, e.pending), e.pending += t;
}
function On(e, r, t, i) {
  var a = r * 2, n = t * 2;
  return e[a] < e[n] || e[a] === e[n] && i[r] <= i[t];
}
function si(e, r, t) {
  for (var i = e.heap[t], a = t << 1; a <= e.heap_len && (a < e.heap_len && On(r, e.heap[a + 1], e.heap[a], e.depth) && a++, !On(r, i, e.heap[a], e.depth)); )
    e.heap[t] = e.heap[a], t = a, a <<= 1;
  e.heap[t] = i;
}
function Dn(e, r, t) {
  var i, a, n = 0, o, s;
  if (e.last_lit !== 0)
    do
      i = e.pending_buf[e.d_buf + n * 2] << 8 | e.pending_buf[e.d_buf + n * 2 + 1], a = e.pending_buf[e.l_buf + n], n++, i === 0 ? Oe(e, a, r) : (o = Zt[a], Oe(e, o + Vt + 1, r), s = xi[o], s !== 0 && (a -= Zi[o], ce(e, a, s)), i--, o = ro(i), Oe(e, o, t), s = pr[o], s !== 0 && (i -= xr[o], ce(e, i, s)));
    while (n < e.last_lit);
  Oe(e, $i, r);
}
function ki(e, r) {
  var t = r.dyn_tree, i = r.stat_desc.static_tree, a = r.stat_desc.has_stree, n = r.stat_desc.elems, o, s, v = -1, f;
  for (e.heap_len = 0, e.heap_max = Ka, o = 0; o < n; o++)
    t[o * 2] !== 0 ? (e.heap[++e.heap_len] = v = o, e.depth[o] = 0) : t[o * 2 + 1] = 0;
  for (; e.heap_len < 2; )
    f = e.heap[++e.heap_len] = v < 2 ? ++v : 0, t[f * 2] = 1, e.depth[f] = 0, e.opt_len--, a && (e.static_len -= i[f * 2 + 1]);
  for (r.max_code = v, o = e.heap_len >> 1; o >= 1; o--)
    si(e, t, o);
  f = n;
  do
    o = e.heap[
      1
      /*SMALLEST*/
    ], e.heap[
      1
      /*SMALLEST*/
    ] = e.heap[e.heap_len--], si(
      e,
      t,
      1
      /*SMALLEST*/
    ), s = e.heap[
      1
      /*SMALLEST*/
    ], e.heap[--e.heap_max] = o, e.heap[--e.heap_max] = s, t[f * 2] = t[o * 2] + t[s * 2], e.depth[f] = (e.depth[o] >= e.depth[s] ? e.depth[o] : e.depth[s]) + 1, t[o * 2 + 1] = t[s * 2 + 1] = f, e.heap[
      1
      /*SMALLEST*/
    ] = f++, si(
      e,
      t,
      1
      /*SMALLEST*/
    );
  while (e.heap_len >= 2);
  e.heap[--e.heap_max] = e.heap[
    1
    /*SMALLEST*/
  ], rf(e, r), no(t, v, e.bl_count);
}
function In(e, r, t) {
  var i, a = -1, n, o = r[0 * 2 + 1], s = 0, v = 7, f = 4;
  for (o === 0 && (v = 138, f = 3), r[(t + 1) * 2 + 1] = 65535, i = 0; i <= t; i++)
    n = o, o = r[(i + 1) * 2 + 1], !(++s < v && n === o) && (s < f ? e.bl_tree[n * 2] += s : n !== 0 ? (n !== a && e.bl_tree[n * 2]++, e.bl_tree[Ga * 2]++) : s <= 10 ? e.bl_tree[Va * 2]++ : e.bl_tree[Xa * 2]++, s = 0, a = n, o === 0 ? (v = 138, f = 3) : n === o ? (v = 6, f = 3) : (v = 7, f = 4));
}
function Bn(e, r, t) {
  var i, a = -1, n, o = r[0 * 2 + 1], s = 0, v = 7, f = 4;
  for (o === 0 && (v = 138, f = 3), i = 0; i <= t; i++)
    if (n = o, o = r[(i + 1) * 2 + 1], !(++s < v && n === o)) {
      if (s < f)
        do
          Oe(e, n, e.bl_tree);
        while (--s !== 0);
      else n !== 0 ? (n !== a && (Oe(e, n, e.bl_tree), s--), Oe(e, Ga, e.bl_tree), ce(e, s - 3, 2)) : s <= 10 ? (Oe(e, Va, e.bl_tree), ce(e, s - 3, 3)) : (Oe(e, Xa, e.bl_tree), ce(e, s - 11, 7));
      s = 0, a = n, o === 0 ? (v = 138, f = 3) : n === o ? (v = 6, f = 3) : (v = 7, f = 4);
    }
}
function of(e) {
  var r;
  for (In(e, e.dyn_ltree, e.l_desc.max_code), In(e, e.dyn_dtree, e.d_desc.max_code), ki(e, e.bl_desc), r = Mi - 1; r >= 3 && e.bl_tree[Ja[r] * 2 + 1] === 0; r--)
    ;
  return e.opt_len += 3 * (r + 1) + 5 + 5 + 4, r;
}
function sf(e, r, t, i) {
  var a;
  for (ce(e, r - 257, 5), ce(e, t - 1, 5), ce(e, i - 4, 4), a = 0; a < i; a++)
    ce(e, e.bl_tree[Ja[a] * 2 + 1], 3);
  Bn(e, e.dyn_ltree, r - 1), Bn(e, e.dyn_dtree, t - 1);
}
function ff(e) {
  var r = 4093624447, t;
  for (t = 0; t <= 31; t++, r >>>= 1)
    if (r & 1 && e.dyn_ltree[t * 2] !== 0)
      return An;
  if (e.dyn_ltree[9 * 2] !== 0 || e.dyn_ltree[10 * 2] !== 0 || e.dyn_ltree[13 * 2] !== 0)
    return Rn;
  for (t = 32; t < Vt; t++)
    if (e.dyn_ltree[t * 2] !== 0)
      return Rn;
  return An;
}
var Fn = !1;
function lf(e) {
  Fn || (nf(), Fn = !0), e.l_desc = new oi(e.dyn_ltree, Qa), e.d_desc = new oi(e.dyn_dtree, eo), e.bl_desc = new oi(e.bl_tree, to), e.bi_buf = 0, e.bi_valid = 0, ao(e);
}
function so(e, r, t, i) {
  ce(e, (Ks << 1) + (i ? 1 : 0), 3), af(e, r, t);
}
function uf(e) {
  ce(e, Ya << 1, 3), Oe(e, $i, Ne), tf(e);
}
function hf(e, r, t, i) {
  var a, n, o = 0;
  e.level > 0 ? (e.strm.data_type === Ys && (e.strm.data_type = ff(e)), ki(e, e.l_desc), ki(e, e.d_desc), o = of(e), a = e.opt_len + 3 + 7 >>> 3, n = e.static_len + 3 + 7 >>> 3, n <= a && (a = n)) : a = n = t + 5, t + 4 <= a && r !== -1 ? so(e, r, t, i) : e.strategy === qs || n === a ? (ce(e, (Ya << 1) + (i ? 1 : 0), 3), Dn(e, Ne, Ft)) : (ce(e, (Gs << 1) + (i ? 1 : 0), 3), sf(e, e.l_desc.max_code + 1, e.d_desc.max_code + 1, o + 1), Dn(e, e.dyn_ltree, e.dyn_dtree)), ao(e), i && oo(e);
}
function df(e, r, t) {
  return e.pending_buf[e.d_buf + e.last_lit * 2] = r >>> 8 & 255, e.pending_buf[e.d_buf + e.last_lit * 2 + 1] = r & 255, e.pending_buf[e.l_buf + e.last_lit] = t & 255, e.last_lit++, r === 0 ? e.dyn_ltree[t * 2]++ : (e.matches++, r--, e.dyn_ltree[(Zt[t] + Vt + 1) * 2]++, e.dyn_dtree[ro(r) * 2]++), e.last_lit === e.lit_bufsize - 1;
}
Et._tr_init = lf;
Et._tr_stored_block = so;
Et._tr_flush_block = hf;
Et._tr_tally = df;
Et._tr_align = uf;
function cf(e, r, t, i) {
  for (var a = e & 65535 | 0, n = e >>> 16 & 65535 | 0, o = 0; t !== 0; ) {
    o = t > 2e3 ? 2e3 : t, t -= o;
    do
      a = a + r[i++] | 0, n = n + a | 0;
    while (--o);
    a %= 65521, n %= 65521;
  }
  return a | n << 16 | 0;
}
var fo = cf;
function vf() {
  for (var e, r = [], t = 0; t < 256; t++) {
    e = t;
    for (var i = 0; i < 8; i++)
      e = e & 1 ? 3988292384 ^ e >>> 1 : e >>> 1;
    r[t] = e;
  }
  return r;
}
var pf = vf();
function _f(e, r, t, i) {
  var a = pf, n = i + t;
  e ^= -1;
  for (var o = i; o < n; o++)
    e = e >>> 8 ^ a[(e ^ r[o]) & 255];
  return e ^ -1;
}
var lo = _f, Wi = {
  2: "need dictionary",
  /* Z_NEED_DICT       2  */
  1: "stream end",
  /* Z_STREAM_END      1  */
  0: "",
  /* Z_OK              0  */
  "-1": "file error",
  /* Z_ERRNO         (-1) */
  "-2": "stream error",
  /* Z_STREAM_ERROR  (-2) */
  "-3": "data error",
  /* Z_DATA_ERROR    (-3) */
  "-4": "insufficient memory",
  /* Z_MEM_ERROR     (-4) */
  "-5": "buffer error",
  /* Z_BUF_ERROR     (-5) */
  "-6": "incompatible version"
  /* Z_VERSION_ERROR (-6) */
}, he = ze, _e = Et, uo = fo, Me = lo, gf = Wi, ot = 0, mf = 1, wf = 3, Ye = 4, Ln = 5, De = 0, Nn = 1, ge = -2, yf = -3, fi = -5, bf = -1, xf = 1, sr = 2, kf = 3, Ef = 4, Sf = 0, Cf = 2, Fr = 8, Tf = 9, Af = 15, Rf = 8, Of = 29, Df = 256, Ei = Df + 1 + Of, If = 30, Bf = 19, Ff = 2 * Ei + 1, Lf = 15, H = 3, He = 258, ke = He + H + 1, Nf = 32, Lr = 42, Si = 69, _r = 73, gr = 91, mr = 103, Je = 113, Bt = 666, ae = 1, Xt = 2, tt = 3, Ct = 4, zf = 3;
function qe(e, r) {
  return e.msg = gf[r], r;
}
function zn(e) {
  return (e << 1) - (e > 4 ? 9 : 0);
}
function We(e) {
  for (var r = e.length; --r >= 0; )
    e[r] = 0;
}
function $e(e) {
  var r = e.state, t = r.pending;
  t > e.avail_out && (t = e.avail_out), t !== 0 && (he.arraySet(e.output, r.pending_buf, r.pending_out, t, e.next_out), e.next_out += t, r.pending_out += t, e.total_out += t, e.avail_out -= t, r.pending -= t, r.pending === 0 && (r.pending_out = 0));
}
function le(e, r) {
  _e._tr_flush_block(e, e.block_start >= 0 ? e.block_start : -1, e.strstart - e.block_start, r), e.block_start = e.strstart, $e(e.strm);
}
function q(e, r) {
  e.pending_buf[e.pending++] = r;
}
function It(e, r) {
  e.pending_buf[e.pending++] = r >>> 8 & 255, e.pending_buf[e.pending++] = r & 255;
}
function Pf(e, r, t, i) {
  var a = e.avail_in;
  return a > i && (a = i), a === 0 ? 0 : (e.avail_in -= a, he.arraySet(r, e.input, e.next_in, a, t), e.state.wrap === 1 ? e.adler = uo(e.adler, r, a, t) : e.state.wrap === 2 && (e.adler = Me(e.adler, r, a, t)), e.next_in += a, e.total_in += a, a);
}
function ho(e, r) {
  var t = e.max_chain_length, i = e.strstart, a, n, o = e.prev_length, s = e.nice_match, v = e.strstart > e.w_size - ke ? e.strstart - (e.w_size - ke) : 0, f = e.window, l = e.w_mask, w = e.prev, h = e.strstart + He, d = f[i + o - 1], g = f[i + o];
  e.prev_length >= e.good_match && (t >>= 2), s > e.lookahead && (s = e.lookahead);
  do
    if (a = r, !(f[a + o] !== g || f[a + o - 1] !== d || f[a] !== f[i] || f[++a] !== f[i + 1])) {
      i += 2, a++;
      do
        ;
      while (f[++i] === f[++a] && f[++i] === f[++a] && f[++i] === f[++a] && f[++i] === f[++a] && f[++i] === f[++a] && f[++i] === f[++a] && f[++i] === f[++a] && f[++i] === f[++a] && i < h);
      if (n = He - (h - i), i = h - He, n > o) {
        if (e.match_start = r, o = n, n >= s)
          break;
        d = f[i + o - 1], g = f[i + o];
      }
    }
  while ((r = w[r & l]) > v && --t !== 0);
  return o <= e.lookahead ? o : e.lookahead;
}
function rt(e) {
  var r = e.w_size, t, i, a, n, o;
  do {
    if (n = e.window_size - e.lookahead - e.strstart, e.strstart >= r + (r - ke)) {
      he.arraySet(e.window, e.window, r, r, 0), e.match_start -= r, e.strstart -= r, e.block_start -= r, i = e.hash_size, t = i;
      do
        a = e.head[--t], e.head[t] = a >= r ? a - r : 0;
      while (--i);
      i = r, t = i;
      do
        a = e.prev[--t], e.prev[t] = a >= r ? a - r : 0;
      while (--i);
      n += r;
    }
    if (e.strm.avail_in === 0)
      break;
    if (i = Pf(e.strm, e.window, e.strstart + e.lookahead, n), e.lookahead += i, e.lookahead + e.insert >= H)
      for (o = e.strstart - e.insert, e.ins_h = e.window[o], e.ins_h = (e.ins_h << e.hash_shift ^ e.window[o + 1]) & e.hash_mask; e.insert && (e.ins_h = (e.ins_h << e.hash_shift ^ e.window[o + H - 1]) & e.hash_mask, e.prev[o & e.w_mask] = e.head[e.ins_h], e.head[e.ins_h] = o, o++, e.insert--, !(e.lookahead + e.insert < H)); )
        ;
  } while (e.lookahead < ke && e.strm.avail_in !== 0);
}
function jf(e, r) {
  var t = 65535;
  for (t > e.pending_buf_size - 5 && (t = e.pending_buf_size - 5); ; ) {
    if (e.lookahead <= 1) {
      if (rt(e), e.lookahead === 0 && r === ot)
        return ae;
      if (e.lookahead === 0)
        break;
    }
    e.strstart += e.lookahead, e.lookahead = 0;
    var i = e.block_start + t;
    if ((e.strstart === 0 || e.strstart >= i) && (e.lookahead = e.strstart - i, e.strstart = i, le(e, !1), e.strm.avail_out === 0) || e.strstart - e.block_start >= e.w_size - ke && (le(e, !1), e.strm.avail_out === 0))
      return ae;
  }
  return e.insert = 0, r === Ye ? (le(e, !0), e.strm.avail_out === 0 ? tt : Ct) : (e.strstart > e.block_start && (le(e, !1), e.strm.avail_out === 0), ae);
}
function li(e, r) {
  for (var t, i; ; ) {
    if (e.lookahead < ke) {
      if (rt(e), e.lookahead < ke && r === ot)
        return ae;
      if (e.lookahead === 0)
        break;
    }
    if (t = 0, e.lookahead >= H && (e.ins_h = (e.ins_h << e.hash_shift ^ e.window[e.strstart + H - 1]) & e.hash_mask, t = e.prev[e.strstart & e.w_mask] = e.head[e.ins_h], e.head[e.ins_h] = e.strstart), t !== 0 && e.strstart - t <= e.w_size - ke && (e.match_length = ho(e, t)), e.match_length >= H)
      if (i = _e._tr_tally(e, e.strstart - e.match_start, e.match_length - H), e.lookahead -= e.match_length, e.match_length <= e.max_lazy_match && e.lookahead >= H) {
        e.match_length--;
        do
          e.strstart++, e.ins_h = (e.ins_h << e.hash_shift ^ e.window[e.strstart + H - 1]) & e.hash_mask, t = e.prev[e.strstart & e.w_mask] = e.head[e.ins_h], e.head[e.ins_h] = e.strstart;
        while (--e.match_length !== 0);
        e.strstart++;
      } else
        e.strstart += e.match_length, e.match_length = 0, e.ins_h = e.window[e.strstart], e.ins_h = (e.ins_h << e.hash_shift ^ e.window[e.strstart + 1]) & e.hash_mask;
    else
      i = _e._tr_tally(e, 0, e.window[e.strstart]), e.lookahead--, e.strstart++;
    if (i && (le(e, !1), e.strm.avail_out === 0))
      return ae;
  }
  return e.insert = e.strstart < H - 1 ? e.strstart : H - 1, r === Ye ? (le(e, !0), e.strm.avail_out === 0 ? tt : Ct) : e.last_lit && (le(e, !1), e.strm.avail_out === 0) ? ae : Xt;
}
function dt(e, r) {
  for (var t, i, a; ; ) {
    if (e.lookahead < ke) {
      if (rt(e), e.lookahead < ke && r === ot)
        return ae;
      if (e.lookahead === 0)
        break;
    }
    if (t = 0, e.lookahead >= H && (e.ins_h = (e.ins_h << e.hash_shift ^ e.window[e.strstart + H - 1]) & e.hash_mask, t = e.prev[e.strstart & e.w_mask] = e.head[e.ins_h], e.head[e.ins_h] = e.strstart), e.prev_length = e.match_length, e.prev_match = e.match_start, e.match_length = H - 1, t !== 0 && e.prev_length < e.max_lazy_match && e.strstart - t <= e.w_size - ke && (e.match_length = ho(e, t), e.match_length <= 5 && (e.strategy === xf || e.match_length === H && e.strstart - e.match_start > 4096) && (e.match_length = H - 1)), e.prev_length >= H && e.match_length <= e.prev_length) {
      a = e.strstart + e.lookahead - H, i = _e._tr_tally(e, e.strstart - 1 - e.prev_match, e.prev_length - H), e.lookahead -= e.prev_length - 1, e.prev_length -= 2;
      do
        ++e.strstart <= a && (e.ins_h = (e.ins_h << e.hash_shift ^ e.window[e.strstart + H - 1]) & e.hash_mask, t = e.prev[e.strstart & e.w_mask] = e.head[e.ins_h], e.head[e.ins_h] = e.strstart);
      while (--e.prev_length !== 0);
      if (e.match_available = 0, e.match_length = H - 1, e.strstart++, i && (le(e, !1), e.strm.avail_out === 0))
        return ae;
    } else if (e.match_available) {
      if (i = _e._tr_tally(e, 0, e.window[e.strstart - 1]), i && le(e, !1), e.strstart++, e.lookahead--, e.strm.avail_out === 0)
        return ae;
    } else
      e.match_available = 1, e.strstart++, e.lookahead--;
  }
  return e.match_available && (i = _e._tr_tally(e, 0, e.window[e.strstart - 1]), e.match_available = 0), e.insert = e.strstart < H - 1 ? e.strstart : H - 1, r === Ye ? (le(e, !0), e.strm.avail_out === 0 ? tt : Ct) : e.last_lit && (le(e, !1), e.strm.avail_out === 0) ? ae : Xt;
}
function Uf(e, r) {
  for (var t, i, a, n, o = e.window; ; ) {
    if (e.lookahead <= He) {
      if (rt(e), e.lookahead <= He && r === ot)
        return ae;
      if (e.lookahead === 0)
        break;
    }
    if (e.match_length = 0, e.lookahead >= H && e.strstart > 0 && (a = e.strstart - 1, i = o[a], i === o[++a] && i === o[++a] && i === o[++a])) {
      n = e.strstart + He;
      do
        ;
      while (i === o[++a] && i === o[++a] && i === o[++a] && i === o[++a] && i === o[++a] && i === o[++a] && i === o[++a] && i === o[++a] && a < n);
      e.match_length = He - (n - a), e.match_length > e.lookahead && (e.match_length = e.lookahead);
    }
    if (e.match_length >= H ? (t = _e._tr_tally(e, 1, e.match_length - H), e.lookahead -= e.match_length, e.strstart += e.match_length, e.match_length = 0) : (t = _e._tr_tally(e, 0, e.window[e.strstart]), e.lookahead--, e.strstart++), t && (le(e, !1), e.strm.avail_out === 0))
      return ae;
  }
  return e.insert = 0, r === Ye ? (le(e, !0), e.strm.avail_out === 0 ? tt : Ct) : e.last_lit && (le(e, !1), e.strm.avail_out === 0) ? ae : Xt;
}
function Mf(e, r) {
  for (var t; ; ) {
    if (e.lookahead === 0 && (rt(e), e.lookahead === 0)) {
      if (r === ot)
        return ae;
      break;
    }
    if (e.match_length = 0, t = _e._tr_tally(e, 0, e.window[e.strstart]), e.lookahead--, e.strstart++, t && (le(e, !1), e.strm.avail_out === 0))
      return ae;
  }
  return e.insert = 0, r === Ye ? (le(e, !0), e.strm.avail_out === 0 ? tt : Ct) : e.last_lit && (le(e, !1), e.strm.avail_out === 0) ? ae : Xt;
}
function Ae(e, r, t, i, a) {
  this.good_length = e, this.max_lazy = r, this.nice_length = t, this.max_chain = i, this.func = a;
}
var pt;
pt = [
  /*      good lazy nice chain */
  new Ae(0, 0, 0, 0, jf),
  /* 0 store only */
  new Ae(4, 4, 8, 4, li),
  /* 1 max speed, no lazy matches */
  new Ae(4, 5, 16, 8, li),
  /* 2 */
  new Ae(4, 6, 32, 32, li),
  /* 3 */
  new Ae(4, 4, 16, 16, dt),
  /* 4 lazy matches */
  new Ae(8, 16, 32, 32, dt),
  /* 5 */
  new Ae(8, 16, 128, 128, dt),
  /* 6 */
  new Ae(8, 32, 128, 256, dt),
  /* 7 */
  new Ae(32, 128, 258, 1024, dt),
  /* 8 */
  new Ae(32, 258, 258, 4096, dt)
  /* 9 max compression */
];
function $f(e) {
  e.window_size = 2 * e.w_size, We(e.head), e.max_lazy_match = pt[e.level].max_lazy, e.good_match = pt[e.level].good_length, e.nice_match = pt[e.level].nice_length, e.max_chain_length = pt[e.level].max_chain, e.strstart = 0, e.block_start = 0, e.lookahead = 0, e.insert = 0, e.match_length = e.prev_length = H - 1, e.match_available = 0, e.ins_h = 0;
}
function Zf() {
  this.strm = null, this.status = 0, this.pending_buf = null, this.pending_buf_size = 0, this.pending_out = 0, this.pending = 0, this.wrap = 0, this.gzhead = null, this.gzindex = 0, this.method = Fr, this.last_flush = -1, this.w_size = 0, this.w_bits = 0, this.w_mask = 0, this.window = null, this.window_size = 0, this.prev = null, this.head = null, this.ins_h = 0, this.hash_size = 0, this.hash_bits = 0, this.hash_mask = 0, this.hash_shift = 0, this.block_start = 0, this.match_length = 0, this.prev_match = 0, this.match_available = 0, this.strstart = 0, this.match_start = 0, this.lookahead = 0, this.prev_length = 0, this.max_chain_length = 0, this.max_lazy_match = 0, this.level = 0, this.strategy = 0, this.good_match = 0, this.nice_match = 0, this.dyn_ltree = new he.Buf16(Ff * 2), this.dyn_dtree = new he.Buf16((2 * If + 1) * 2), this.bl_tree = new he.Buf16((2 * Bf + 1) * 2), We(this.dyn_ltree), We(this.dyn_dtree), We(this.bl_tree), this.l_desc = null, this.d_desc = null, this.bl_desc = null, this.bl_count = new he.Buf16(Lf + 1), this.heap = new he.Buf16(2 * Ei + 1), We(this.heap), this.heap_len = 0, this.heap_max = 0, this.depth = new he.Buf16(2 * Ei + 1), We(this.depth), this.l_buf = 0, this.lit_bufsize = 0, this.last_lit = 0, this.d_buf = 0, this.opt_len = 0, this.static_len = 0, this.matches = 0, this.insert = 0, this.bi_buf = 0, this.bi_valid = 0;
}
function co(e) {
  var r;
  return !e || !e.state ? qe(e, ge) : (e.total_in = e.total_out = 0, e.data_type = Cf, r = e.state, r.pending = 0, r.pending_out = 0, r.wrap < 0 && (r.wrap = -r.wrap), r.status = r.wrap ? Lr : Je, e.adler = r.wrap === 2 ? 0 : 1, r.last_flush = ot, _e._tr_init(r), De);
}
function vo(e) {
  var r = co(e);
  return r === De && $f(e.state), r;
}
function Wf(e, r) {
  return !e || !e.state || e.state.wrap !== 2 ? ge : (e.state.gzhead = r, De);
}
function po(e, r, t, i, a, n) {
  if (!e)
    return ge;
  var o = 1;
  if (r === bf && (r = 6), i < 0 ? (o = 0, i = -i) : i > 15 && (o = 2, i -= 16), a < 1 || a > Tf || t !== Fr || i < 8 || i > 15 || r < 0 || r > 9 || n < 0 || n > Ef)
    return qe(e, ge);
  i === 8 && (i = 9);
  var s = new Zf();
  return e.state = s, s.strm = e, s.wrap = o, s.gzhead = null, s.w_bits = i, s.w_size = 1 << s.w_bits, s.w_mask = s.w_size - 1, s.hash_bits = a + 7, s.hash_size = 1 << s.hash_bits, s.hash_mask = s.hash_size - 1, s.hash_shift = ~~((s.hash_bits + H - 1) / H), s.window = new he.Buf8(s.w_size * 2), s.head = new he.Buf16(s.hash_size), s.prev = new he.Buf16(s.w_size), s.lit_bufsize = 1 << a + 6, s.pending_buf_size = s.lit_bufsize * 4, s.pending_buf = new he.Buf8(s.pending_buf_size), s.d_buf = 1 * s.lit_bufsize, s.l_buf = 3 * s.lit_bufsize, s.level = r, s.strategy = n, s.method = t, vo(e);
}
function Hf(e, r) {
  return po(e, r, Fr, Af, Rf, Sf);
}
function qf(e, r) {
  var t, i, a, n;
  if (!e || !e.state || r > Ln || r < 0)
    return e ? qe(e, ge) : ge;
  if (i = e.state, !e.output || !e.input && e.avail_in !== 0 || i.status === Bt && r !== Ye)
    return qe(e, e.avail_out === 0 ? fi : ge);
  if (i.strm = e, t = i.last_flush, i.last_flush = r, i.status === Lr)
    if (i.wrap === 2)
      e.adler = 0, q(i, 31), q(i, 139), q(i, 8), i.gzhead ? (q(
        i,
        (i.gzhead.text ? 1 : 0) + (i.gzhead.hcrc ? 2 : 0) + (i.gzhead.extra ? 4 : 0) + (i.gzhead.name ? 8 : 0) + (i.gzhead.comment ? 16 : 0)
      ), q(i, i.gzhead.time & 255), q(i, i.gzhead.time >> 8 & 255), q(i, i.gzhead.time >> 16 & 255), q(i, i.gzhead.time >> 24 & 255), q(i, i.level === 9 ? 2 : i.strategy >= sr || i.level < 2 ? 4 : 0), q(i, i.gzhead.os & 255), i.gzhead.extra && i.gzhead.extra.length && (q(i, i.gzhead.extra.length & 255), q(i, i.gzhead.extra.length >> 8 & 255)), i.gzhead.hcrc && (e.adler = Me(e.adler, i.pending_buf, i.pending, 0)), i.gzindex = 0, i.status = Si) : (q(i, 0), q(i, 0), q(i, 0), q(i, 0), q(i, 0), q(i, i.level === 9 ? 2 : i.strategy >= sr || i.level < 2 ? 4 : 0), q(i, zf), i.status = Je);
    else {
      var o = Fr + (i.w_bits - 8 << 4) << 8, s = -1;
      i.strategy >= sr || i.level < 2 ? s = 0 : i.level < 6 ? s = 1 : i.level === 6 ? s = 2 : s = 3, o |= s << 6, i.strstart !== 0 && (o |= Nf), o += 31 - o % 31, i.status = Je, It(i, o), i.strstart !== 0 && (It(i, e.adler >>> 16), It(i, e.adler & 65535)), e.adler = 1;
    }
  if (i.status === Si)
    if (i.gzhead.extra) {
      for (a = i.pending; i.gzindex < (i.gzhead.extra.length & 65535) && !(i.pending === i.pending_buf_size && (i.gzhead.hcrc && i.pending > a && (e.adler = Me(e.adler, i.pending_buf, i.pending - a, a)), $e(e), a = i.pending, i.pending === i.pending_buf_size)); )
        q(i, i.gzhead.extra[i.gzindex] & 255), i.gzindex++;
      i.gzhead.hcrc && i.pending > a && (e.adler = Me(e.adler, i.pending_buf, i.pending - a, a)), i.gzindex === i.gzhead.extra.length && (i.gzindex = 0, i.status = _r);
    } else
      i.status = _r;
  if (i.status === _r)
    if (i.gzhead.name) {
      a = i.pending;
      do {
        if (i.pending === i.pending_buf_size && (i.gzhead.hcrc && i.pending > a && (e.adler = Me(e.adler, i.pending_buf, i.pending - a, a)), $e(e), a = i.pending, i.pending === i.pending_buf_size)) {
          n = 1;
          break;
        }
        i.gzindex < i.gzhead.name.length ? n = i.gzhead.name.charCodeAt(i.gzindex++) & 255 : n = 0, q(i, n);
      } while (n !== 0);
      i.gzhead.hcrc && i.pending > a && (e.adler = Me(e.adler, i.pending_buf, i.pending - a, a)), n === 0 && (i.gzindex = 0, i.status = gr);
    } else
      i.status = gr;
  if (i.status === gr)
    if (i.gzhead.comment) {
      a = i.pending;
      do {
        if (i.pending === i.pending_buf_size && (i.gzhead.hcrc && i.pending > a && (e.adler = Me(e.adler, i.pending_buf, i.pending - a, a)), $e(e), a = i.pending, i.pending === i.pending_buf_size)) {
          n = 1;
          break;
        }
        i.gzindex < i.gzhead.comment.length ? n = i.gzhead.comment.charCodeAt(i.gzindex++) & 255 : n = 0, q(i, n);
      } while (n !== 0);
      i.gzhead.hcrc && i.pending > a && (e.adler = Me(e.adler, i.pending_buf, i.pending - a, a)), n === 0 && (i.status = mr);
    } else
      i.status = mr;
  if (i.status === mr && (i.gzhead.hcrc ? (i.pending + 2 > i.pending_buf_size && $e(e), i.pending + 2 <= i.pending_buf_size && (q(i, e.adler & 255), q(i, e.adler >> 8 & 255), e.adler = 0, i.status = Je)) : i.status = Je), i.pending !== 0) {
    if ($e(e), e.avail_out === 0)
      return i.last_flush = -1, De;
  } else if (e.avail_in === 0 && zn(r) <= zn(t) && r !== Ye)
    return qe(e, fi);
  if (i.status === Bt && e.avail_in !== 0)
    return qe(e, fi);
  if (e.avail_in !== 0 || i.lookahead !== 0 || r !== ot && i.status !== Bt) {
    var v = i.strategy === sr ? Mf(i, r) : i.strategy === kf ? Uf(i, r) : pt[i.level].func(i, r);
    if ((v === tt || v === Ct) && (i.status = Bt), v === ae || v === tt)
      return e.avail_out === 0 && (i.last_flush = -1), De;
    if (v === Xt && (r === mf ? _e._tr_align(i) : r !== Ln && (_e._tr_stored_block(i, 0, 0, !1), r === wf && (We(i.head), i.lookahead === 0 && (i.strstart = 0, i.block_start = 0, i.insert = 0))), $e(e), e.avail_out === 0))
      return i.last_flush = -1, De;
  }
  return r !== Ye ? De : i.wrap <= 0 ? Nn : (i.wrap === 2 ? (q(i, e.adler & 255), q(i, e.adler >> 8 & 255), q(i, e.adler >> 16 & 255), q(i, e.adler >> 24 & 255), q(i, e.total_in & 255), q(i, e.total_in >> 8 & 255), q(i, e.total_in >> 16 & 255), q(i, e.total_in >> 24 & 255)) : (It(i, e.adler >>> 16), It(i, e.adler & 65535)), $e(e), i.wrap > 0 && (i.wrap = -i.wrap), i.pending !== 0 ? De : Nn);
}
function Yf(e) {
  var r;
  return !e || !e.state ? ge : (r = e.state.status, r !== Lr && r !== Si && r !== _r && r !== gr && r !== mr && r !== Je && r !== Bt ? qe(e, ge) : (e.state = null, r === Je ? qe(e, yf) : De));
}
function Kf(e, r) {
  var t = r.length, i, a, n, o, s, v, f, l;
  if (!e || !e.state || (i = e.state, o = i.wrap, o === 2 || o === 1 && i.status !== Lr || i.lookahead))
    return ge;
  for (o === 1 && (e.adler = uo(e.adler, r, t, 0)), i.wrap = 0, t >= i.w_size && (o === 0 && (We(i.head), i.strstart = 0, i.block_start = 0, i.insert = 0), l = new he.Buf8(i.w_size), he.arraySet(l, r, t - i.w_size, i.w_size, 0), r = l, t = i.w_size), s = e.avail_in, v = e.next_in, f = e.input, e.avail_in = t, e.next_in = 0, e.input = r, rt(i); i.lookahead >= H; ) {
    a = i.strstart, n = i.lookahead - (H - 1);
    do
      i.ins_h = (i.ins_h << i.hash_shift ^ i.window[a + H - 1]) & i.hash_mask, i.prev[a & i.w_mask] = i.head[i.ins_h], i.head[i.ins_h] = a, a++;
    while (--n);
    i.strstart = a, i.lookahead = H - 1, rt(i);
  }
  return i.strstart += i.lookahead, i.block_start = i.strstart, i.insert = i.lookahead, i.lookahead = 0, i.match_length = i.prev_length = H - 1, i.match_available = 0, e.next_in = v, e.input = f, e.avail_in = s, i.wrap = o, De;
}
Ie.deflateInit = Hf;
Ie.deflateInit2 = po;
Ie.deflateReset = vo;
Ie.deflateResetKeep = co;
Ie.deflateSetHeader = Wf;
Ie.deflate = qf;
Ie.deflateEnd = Yf;
Ie.deflateSetDictionary = Kf;
Ie.deflateInfo = "pako deflate (from Nodeca project)";
var st = {}, Nr = ze, _o = !0, go = !0;
try {
  String.fromCharCode.apply(null, [0]);
} catch {
  _o = !1;
}
try {
  String.fromCharCode.apply(null, new Uint8Array(1));
} catch {
  go = !1;
}
var Ht = new Nr.Buf8(256);
for (var je = 0; je < 256; je++)
  Ht[je] = je >= 252 ? 6 : je >= 248 ? 5 : je >= 240 ? 4 : je >= 224 ? 3 : je >= 192 ? 2 : 1;
Ht[254] = Ht[254] = 1;
st.string2buf = function(e) {
  var r, t, i, a, n, o = e.length, s = 0;
  for (a = 0; a < o; a++)
    t = e.charCodeAt(a), (t & 64512) === 55296 && a + 1 < o && (i = e.charCodeAt(a + 1), (i & 64512) === 56320 && (t = 65536 + (t - 55296 << 10) + (i - 56320), a++)), s += t < 128 ? 1 : t < 2048 ? 2 : t < 65536 ? 3 : 4;
  for (r = new Nr.Buf8(s), n = 0, a = 0; n < s; a++)
    t = e.charCodeAt(a), (t & 64512) === 55296 && a + 1 < o && (i = e.charCodeAt(a + 1), (i & 64512) === 56320 && (t = 65536 + (t - 55296 << 10) + (i - 56320), a++)), t < 128 ? r[n++] = t : t < 2048 ? (r[n++] = 192 | t >>> 6, r[n++] = 128 | t & 63) : t < 65536 ? (r[n++] = 224 | t >>> 12, r[n++] = 128 | t >>> 6 & 63, r[n++] = 128 | t & 63) : (r[n++] = 240 | t >>> 18, r[n++] = 128 | t >>> 12 & 63, r[n++] = 128 | t >>> 6 & 63, r[n++] = 128 | t & 63);
  return r;
};
function mo(e, r) {
  if (r < 65534 && (e.subarray && go || !e.subarray && _o))
    return String.fromCharCode.apply(null, Nr.shrinkBuf(e, r));
  for (var t = "", i = 0; i < r; i++)
    t += String.fromCharCode(e[i]);
  return t;
}
st.buf2binstring = function(e) {
  return mo(e, e.length);
};
st.binstring2buf = function(e) {
  for (var r = new Nr.Buf8(e.length), t = 0, i = r.length; t < i; t++)
    r[t] = e.charCodeAt(t);
  return r;
};
st.buf2string = function(e, r) {
  var t, i, a, n, o = r || e.length, s = new Array(o * 2);
  for (i = 0, t = 0; t < o; ) {
    if (a = e[t++], a < 128) {
      s[i++] = a;
      continue;
    }
    if (n = Ht[a], n > 4) {
      s[i++] = 65533, t += n - 1;
      continue;
    }
    for (a &= n === 2 ? 31 : n === 3 ? 15 : 7; n > 1 && t < o; )
      a = a << 6 | e[t++] & 63, n--;
    if (n > 1) {
      s[i++] = 65533;
      continue;
    }
    a < 65536 ? s[i++] = a : (a -= 65536, s[i++] = 55296 | a >> 10 & 1023, s[i++] = 56320 | a & 1023);
  }
  return mo(s, i);
};
st.utf8border = function(e, r) {
  var t;
  for (r = r || e.length, r > e.length && (r = e.length), t = r - 1; t >= 0 && (e[t] & 192) === 128; )
    t--;
  return t < 0 || t === 0 ? r : t + Ht[e[t]] > r ? t : r;
};
function Gf() {
  this.input = null, this.next_in = 0, this.avail_in = 0, this.total_in = 0, this.output = null, this.next_out = 0, this.avail_out = 0, this.total_out = 0, this.msg = "", this.state = null, this.data_type = 2, this.adler = 0;
}
var wo = Gf, Lt = Ie, Nt = ze, Ci = st, Ti = Wi, Vf = wo, yo = Object.prototype.toString, Xf = 0, ui = 4, mt = 0, Pn = 1, jn = 2, Jf = -1, Qf = 0, el = 8;
function it(e) {
  if (!(this instanceof it)) return new it(e);
  this.options = Nt.assign({
    level: Jf,
    method: el,
    chunkSize: 16384,
    windowBits: 15,
    memLevel: 8,
    strategy: Qf,
    to: ""
  }, e || {});
  var r = this.options;
  r.raw && r.windowBits > 0 ? r.windowBits = -r.windowBits : r.gzip && r.windowBits > 0 && r.windowBits < 16 && (r.windowBits += 16), this.err = 0, this.msg = "", this.ended = !1, this.chunks = [], this.strm = new Vf(), this.strm.avail_out = 0;
  var t = Lt.deflateInit2(
    this.strm,
    r.level,
    r.method,
    r.windowBits,
    r.memLevel,
    r.strategy
  );
  if (t !== mt)
    throw new Error(Ti[t]);
  if (r.header && Lt.deflateSetHeader(this.strm, r.header), r.dictionary) {
    var i;
    if (typeof r.dictionary == "string" ? i = Ci.string2buf(r.dictionary) : yo.call(r.dictionary) === "[object ArrayBuffer]" ? i = new Uint8Array(r.dictionary) : i = r.dictionary, t = Lt.deflateSetDictionary(this.strm, i), t !== mt)
      throw new Error(Ti[t]);
    this._dict_set = !0;
  }
}
it.prototype.push = function(e, r) {
  var t = this.strm, i = this.options.chunkSize, a, n;
  if (this.ended)
    return !1;
  n = r === ~~r ? r : r === !0 ? ui : Xf, typeof e == "string" ? t.input = Ci.string2buf(e) : yo.call(e) === "[object ArrayBuffer]" ? t.input = new Uint8Array(e) : t.input = e, t.next_in = 0, t.avail_in = t.input.length;
  do {
    if (t.avail_out === 0 && (t.output = new Nt.Buf8(i), t.next_out = 0, t.avail_out = i), a = Lt.deflate(t, n), a !== Pn && a !== mt)
      return this.onEnd(a), this.ended = !0, !1;
    (t.avail_out === 0 || t.avail_in === 0 && (n === ui || n === jn)) && (this.options.to === "string" ? this.onData(Ci.buf2binstring(Nt.shrinkBuf(t.output, t.next_out))) : this.onData(Nt.shrinkBuf(t.output, t.next_out)));
  } while ((t.avail_in > 0 || t.avail_out === 0) && a !== Pn);
  return n === ui ? (a = Lt.deflateEnd(this.strm), this.onEnd(a), this.ended = !0, a === mt) : (n === jn && (this.onEnd(mt), t.avail_out = 0), !0);
};
it.prototype.onData = function(e) {
  this.chunks.push(e);
};
it.prototype.onEnd = function(e) {
  e === mt && (this.options.to === "string" ? this.result = this.chunks.join("") : this.result = Nt.flattenChunks(this.chunks)), this.chunks = [], this.err = e, this.msg = this.strm.msg;
};
function Hi(e, r) {
  var t = new it(r);
  if (t.push(e, !0), t.err)
    throw t.msg || Ti[t.err];
  return t.result;
}
function tl(e, r) {
  return r = r || {}, r.raw = !0, Hi(e, r);
}
function rl(e, r) {
  return r = r || {}, r.gzip = !0, Hi(e, r);
}
Gt.Deflate = it;
Gt.deflate = Hi;
Gt.deflateRaw = tl;
Gt.gzip = rl;
var Jt = {}, Ee = {}, fr = 30, il = 12, nl = function(r, t) {
  var i, a, n, o, s, v, f, l, w, h, d, g, m, C, u, _, y, E, S, F, L, B, z, M, D;
  i = r.state, a = r.next_in, M = r.input, n = a + (r.avail_in - 5), o = r.next_out, D = r.output, s = o - (t - r.avail_out), v = o + (r.avail_out - 257), f = i.dmax, l = i.wsize, w = i.whave, h = i.wnext, d = i.window, g = i.hold, m = i.bits, C = i.lencode, u = i.distcode, _ = (1 << i.lenbits) - 1, y = (1 << i.distbits) - 1;
  e:
    do {
      m < 15 && (g += M[a++] << m, m += 8, g += M[a++] << m, m += 8), E = C[g & _];
      t:
        for (; ; ) {
          if (S = E >>> 24, g >>>= S, m -= S, S = E >>> 16 & 255, S === 0)
            D[o++] = E & 65535;
          else if (S & 16) {
            F = E & 65535, S &= 15, S && (m < S && (g += M[a++] << m, m += 8), F += g & (1 << S) - 1, g >>>= S, m -= S), m < 15 && (g += M[a++] << m, m += 8, g += M[a++] << m, m += 8), E = u[g & y];
            r:
              for (; ; ) {
                if (S = E >>> 24, g >>>= S, m -= S, S = E >>> 16 & 255, S & 16) {
                  if (L = E & 65535, S &= 15, m < S && (g += M[a++] << m, m += 8, m < S && (g += M[a++] << m, m += 8)), L += g & (1 << S) - 1, L > f) {
                    r.msg = "invalid distance too far back", i.mode = fr;
                    break e;
                  }
                  if (g >>>= S, m -= S, S = o - s, L > S) {
                    if (S = L - S, S > w && i.sane) {
                      r.msg = "invalid distance too far back", i.mode = fr;
                      break e;
                    }
                    if (B = 0, z = d, h === 0) {
                      if (B += l - S, S < F) {
                        F -= S;
                        do
                          D[o++] = d[B++];
                        while (--S);
                        B = o - L, z = D;
                      }
                    } else if (h < S) {
                      if (B += l + h - S, S -= h, S < F) {
                        F -= S;
                        do
                          D[o++] = d[B++];
                        while (--S);
                        if (B = 0, h < F) {
                          S = h, F -= S;
                          do
                            D[o++] = d[B++];
                          while (--S);
                          B = o - L, z = D;
                        }
                      }
                    } else if (B += h - S, S < F) {
                      F -= S;
                      do
                        D[o++] = d[B++];
                      while (--S);
                      B = o - L, z = D;
                    }
                    for (; F > 2; )
                      D[o++] = z[B++], D[o++] = z[B++], D[o++] = z[B++], F -= 3;
                    F && (D[o++] = z[B++], F > 1 && (D[o++] = z[B++]));
                  } else {
                    B = o - L;
                    do
                      D[o++] = D[B++], D[o++] = D[B++], D[o++] = D[B++], F -= 3;
                    while (F > 2);
                    F && (D[o++] = D[B++], F > 1 && (D[o++] = D[B++]));
                  }
                } else if (S & 64) {
                  r.msg = "invalid distance code", i.mode = fr;
                  break e;
                } else {
                  E = u[(E & 65535) + (g & (1 << S) - 1)];
                  continue r;
                }
                break;
              }
          } else if (S & 64)
            if (S & 32) {
              i.mode = il;
              break e;
            } else {
              r.msg = "invalid literal/length code", i.mode = fr;
              break e;
            }
          else {
            E = C[(E & 65535) + (g & (1 << S) - 1)];
            continue t;
          }
          break;
        }
    } while (a < n && o < v);
  F = m >> 3, a -= F, m -= F << 3, g &= (1 << m) - 1, r.next_in = a, r.next_out = o, r.avail_in = a < n ? 5 + (n - a) : 5 - (a - n), r.avail_out = o < v ? 257 + (v - o) : 257 - (o - v), i.hold = g, i.bits = m;
}, Un = ze, ct = 15, Mn = 852, $n = 592, Zn = 0, hi = 1, Wn = 2, al = [
  /* Length codes 257..285 base */
  3,
  4,
  5,
  6,
  7,
  8,
  9,
  10,
  11,
  13,
  15,
  17,
  19,
  23,
  27,
  31,
  35,
  43,
  51,
  59,
  67,
  83,
  99,
  115,
  131,
  163,
  195,
  227,
  258,
  0,
  0
], ol = [
  /* Length codes 257..285 extra */
  16,
  16,
  16,
  16,
  16,
  16,
  16,
  16,
  17,
  17,
  17,
  17,
  18,
  18,
  18,
  18,
  19,
  19,
  19,
  19,
  20,
  20,
  20,
  20,
  21,
  21,
  21,
  21,
  16,
  72,
  78
], sl = [
  /* Distance codes 0..29 base */
  1,
  2,
  3,
  4,
  5,
  7,
  9,
  13,
  17,
  25,
  33,
  49,
  65,
  97,
  129,
  193,
  257,
  385,
  513,
  769,
  1025,
  1537,
  2049,
  3073,
  4097,
  6145,
  8193,
  12289,
  16385,
  24577,
  0,
  0
], fl = [
  /* Distance codes 0..29 extra */
  16,
  16,
  16,
  16,
  17,
  17,
  18,
  18,
  19,
  19,
  20,
  20,
  21,
  21,
  22,
  22,
  23,
  23,
  24,
  24,
  25,
  25,
  26,
  26,
  27,
  27,
  28,
  28,
  29,
  29,
  64,
  64
], ll = function(r, t, i, a, n, o, s, v) {
  var f = v.bits, l = 0, w = 0, h = 0, d = 0, g = 0, m = 0, C = 0, u = 0, _ = 0, y = 0, E, S, F, L, B, z = null, M = 0, D, X = new Un.Buf16(ct + 1), se = new Un.Buf16(ct + 1), $ = null, Be = 0, Pe, x, b;
  for (l = 0; l <= ct; l++)
    X[l] = 0;
  for (w = 0; w < a; w++)
    X[t[i + w]]++;
  for (g = f, d = ct; d >= 1 && X[d] === 0; d--)
    ;
  if (g > d && (g = d), d === 0)
    return n[o++] = 1 << 24 | 64 << 16 | 0, n[o++] = 1 << 24 | 64 << 16 | 0, v.bits = 1, 0;
  for (h = 1; h < d && X[h] === 0; h++)
    ;
  for (g < h && (g = h), u = 1, l = 1; l <= ct; l++)
    if (u <<= 1, u -= X[l], u < 0)
      return -1;
  if (u > 0 && (r === Zn || d !== 1))
    return -1;
  for (se[1] = 0, l = 1; l < ct; l++)
    se[l + 1] = se[l] + X[l];
  for (w = 0; w < a; w++)
    t[i + w] !== 0 && (s[se[t[i + w]]++] = w);
  if (r === Zn ? (z = $ = s, D = 19) : r === hi ? (z = al, M -= 257, $ = ol, Be -= 257, D = 256) : (z = sl, $ = fl, D = -1), y = 0, w = 0, l = h, B = o, m = g, C = 0, F = -1, _ = 1 << g, L = _ - 1, r === hi && _ > Mn || r === Wn && _ > $n)
    return 1;
  for (; ; ) {
    Pe = l - C, s[w] < D ? (x = 0, b = s[w]) : s[w] > D ? (x = $[Be + s[w]], b = z[M + s[w]]) : (x = 96, b = 0), E = 1 << l - C, S = 1 << m, h = S;
    do
      S -= E, n[B + (y >> C) + S] = Pe << 24 | x << 16 | b | 0;
    while (S !== 0);
    for (E = 1 << l - 1; y & E; )
      E >>= 1;
    if (E !== 0 ? (y &= E - 1, y += E) : y = 0, w++, --X[l] === 0) {
      if (l === d)
        break;
      l = t[i + s[w]];
    }
    if (l > g && (y & L) !== F) {
      for (C === 0 && (C = g), B += h, m = l - C, u = 1 << m; m + C < d && (u -= X[m + C], !(u <= 0)); )
        m++, u <<= 1;
      if (_ += 1 << m, r === hi && _ > Mn || r === Wn && _ > $n)
        return 1;
      F = y & L, n[F] = g << 24 | m << 16 | B - o | 0;
    }
  }
  return y !== 0 && (n[B + y] = l - C << 24 | 64 << 16 | 0), v.bits = g, 0;
}, ve = ze, Ai = fo, Re = lo, ul = nl, zt = ll, hl = 0, bo = 1, xo = 2, Hn = 4, dl = 5, lr = 6, nt = 0, cl = 1, vl = 2, we = -2, ko = -3, Eo = -4, pl = -5, qn = 8, So = 1, Yn = 2, Kn = 3, Gn = 4, Vn = 5, Xn = 6, Jn = 7, Qn = 8, ea = 9, ta = 10, kr = 11, Fe = 12, di = 13, ra = 14, ci = 15, ia = 16, na = 17, aa = 18, oa = 19, ur = 20, hr = 21, sa = 22, fa = 23, la = 24, ua = 25, ha = 26, vi = 27, da = 28, ca = 29, J = 30, Co = 31, _l = 32, gl = 852, ml = 592, wl = 15, yl = wl;
function va(e) {
  return (e >>> 24 & 255) + (e >>> 8 & 65280) + ((e & 65280) << 8) + ((e & 255) << 24);
}
function bl() {
  this.mode = 0, this.last = !1, this.wrap = 0, this.havedict = !1, this.flags = 0, this.dmax = 0, this.check = 0, this.total = 0, this.head = null, this.wbits = 0, this.wsize = 0, this.whave = 0, this.wnext = 0, this.window = null, this.hold = 0, this.bits = 0, this.length = 0, this.offset = 0, this.extra = 0, this.lencode = null, this.distcode = null, this.lenbits = 0, this.distbits = 0, this.ncode = 0, this.nlen = 0, this.ndist = 0, this.have = 0, this.next = null, this.lens = new ve.Buf16(320), this.work = new ve.Buf16(288), this.lendyn = null, this.distdyn = null, this.sane = 0, this.back = 0, this.was = 0;
}
function To(e) {
  var r;
  return !e || !e.state ? we : (r = e.state, e.total_in = e.total_out = r.total = 0, e.msg = "", r.wrap && (e.adler = r.wrap & 1), r.mode = So, r.last = 0, r.havedict = 0, r.dmax = 32768, r.head = null, r.hold = 0, r.bits = 0, r.lencode = r.lendyn = new ve.Buf32(gl), r.distcode = r.distdyn = new ve.Buf32(ml), r.sane = 1, r.back = -1, nt);
}
function Ao(e) {
  var r;
  return !e || !e.state ? we : (r = e.state, r.wsize = 0, r.whave = 0, r.wnext = 0, To(e));
}
function Ro(e, r) {
  var t, i;
  return !e || !e.state || (i = e.state, r < 0 ? (t = 0, r = -r) : (t = (r >> 4) + 1, r < 48 && (r &= 15)), r && (r < 8 || r > 15)) ? we : (i.window !== null && i.wbits !== r && (i.window = null), i.wrap = t, i.wbits = r, Ao(e));
}
function Oo(e, r) {
  var t, i;
  return e ? (i = new bl(), e.state = i, i.window = null, t = Ro(e, r), t !== nt && (e.state = null), t) : we;
}
function xl(e) {
  return Oo(e, yl);
}
var pa = !0, pi, _i;
function kl(e) {
  if (pa) {
    var r;
    for (pi = new ve.Buf32(512), _i = new ve.Buf32(32), r = 0; r < 144; )
      e.lens[r++] = 8;
    for (; r < 256; )
      e.lens[r++] = 9;
    for (; r < 280; )
      e.lens[r++] = 7;
    for (; r < 288; )
      e.lens[r++] = 8;
    for (zt(bo, e.lens, 0, 288, pi, 0, e.work, { bits: 9 }), r = 0; r < 32; )
      e.lens[r++] = 5;
    zt(xo, e.lens, 0, 32, _i, 0, e.work, { bits: 5 }), pa = !1;
  }
  e.lencode = pi, e.lenbits = 9, e.distcode = _i, e.distbits = 5;
}
function Do(e, r, t, i) {
  var a, n = e.state;
  return n.window === null && (n.wsize = 1 << n.wbits, n.wnext = 0, n.whave = 0, n.window = new ve.Buf8(n.wsize)), i >= n.wsize ? (ve.arraySet(n.window, r, t - n.wsize, n.wsize, 0), n.wnext = 0, n.whave = n.wsize) : (a = n.wsize - n.wnext, a > i && (a = i), ve.arraySet(n.window, r, t - i, a, n.wnext), i -= a, i ? (ve.arraySet(n.window, r, t - i, i, 0), n.wnext = i, n.whave = n.wsize) : (n.wnext += a, n.wnext === n.wsize && (n.wnext = 0), n.whave < n.wsize && (n.whave += a))), 0;
}
function El(e, r) {
  var t, i, a, n, o, s, v, f, l, w, h, d, g, m, C = 0, u, _, y, E, S, F, L, B, z = new ve.Buf8(4), M, D, X = (
    /* permutation of code lengths */
    [16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15]
  );
  if (!e || !e.state || !e.output || !e.input && e.avail_in !== 0)
    return we;
  t = e.state, t.mode === Fe && (t.mode = di), o = e.next_out, a = e.output, v = e.avail_out, n = e.next_in, i = e.input, s = e.avail_in, f = t.hold, l = t.bits, w = s, h = v, B = nt;
  e:
    for (; ; )
      switch (t.mode) {
        case So:
          if (t.wrap === 0) {
            t.mode = di;
            break;
          }
          for (; l < 16; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          if (t.wrap & 2 && f === 35615) {
            t.check = 0, z[0] = f & 255, z[1] = f >>> 8 & 255, t.check = Re(t.check, z, 2, 0), f = 0, l = 0, t.mode = Yn;
            break;
          }
          if (t.flags = 0, t.head && (t.head.done = !1), !(t.wrap & 1) || /* check if zlib header allowed */
          (((f & 255) << 8) + (f >> 8)) % 31) {
            e.msg = "incorrect header check", t.mode = J;
            break;
          }
          if ((f & 15) !== qn) {
            e.msg = "unknown compression method", t.mode = J;
            break;
          }
          if (f >>>= 4, l -= 4, L = (f & 15) + 8, t.wbits === 0)
            t.wbits = L;
          else if (L > t.wbits) {
            e.msg = "invalid window size", t.mode = J;
            break;
          }
          t.dmax = 1 << L, e.adler = t.check = 1, t.mode = f & 512 ? ta : Fe, f = 0, l = 0;
          break;
        case Yn:
          for (; l < 16; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          if (t.flags = f, (t.flags & 255) !== qn) {
            e.msg = "unknown compression method", t.mode = J;
            break;
          }
          if (t.flags & 57344) {
            e.msg = "unknown header flags set", t.mode = J;
            break;
          }
          t.head && (t.head.text = f >> 8 & 1), t.flags & 512 && (z[0] = f & 255, z[1] = f >>> 8 & 255, t.check = Re(t.check, z, 2, 0)), f = 0, l = 0, t.mode = Kn;
        case Kn:
          for (; l < 32; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          t.head && (t.head.time = f), t.flags & 512 && (z[0] = f & 255, z[1] = f >>> 8 & 255, z[2] = f >>> 16 & 255, z[3] = f >>> 24 & 255, t.check = Re(t.check, z, 4, 0)), f = 0, l = 0, t.mode = Gn;
        case Gn:
          for (; l < 16; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          t.head && (t.head.xflags = f & 255, t.head.os = f >> 8), t.flags & 512 && (z[0] = f & 255, z[1] = f >>> 8 & 255, t.check = Re(t.check, z, 2, 0)), f = 0, l = 0, t.mode = Vn;
        case Vn:
          if (t.flags & 1024) {
            for (; l < 16; ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            t.length = f, t.head && (t.head.extra_len = f), t.flags & 512 && (z[0] = f & 255, z[1] = f >>> 8 & 255, t.check = Re(t.check, z, 2, 0)), f = 0, l = 0;
          } else t.head && (t.head.extra = null);
          t.mode = Xn;
        case Xn:
          if (t.flags & 1024 && (d = t.length, d > s && (d = s), d && (t.head && (L = t.head.extra_len - t.length, t.head.extra || (t.head.extra = new Array(t.head.extra_len)), ve.arraySet(
            t.head.extra,
            i,
            n,
            // extra field is limited to 65536 bytes
            // - no need for additional size check
            d,
            /*len + copy > state.head.extra_max - len ? state.head.extra_max : copy,*/
            L
          )), t.flags & 512 && (t.check = Re(t.check, i, d, n)), s -= d, n += d, t.length -= d), t.length))
            break e;
          t.length = 0, t.mode = Jn;
        case Jn:
          if (t.flags & 2048) {
            if (s === 0)
              break e;
            d = 0;
            do
              L = i[n + d++], t.head && L && t.length < 65536 && (t.head.name += String.fromCharCode(L));
            while (L && d < s);
            if (t.flags & 512 && (t.check = Re(t.check, i, d, n)), s -= d, n += d, L)
              break e;
          } else t.head && (t.head.name = null);
          t.length = 0, t.mode = Qn;
        case Qn:
          if (t.flags & 4096) {
            if (s === 0)
              break e;
            d = 0;
            do
              L = i[n + d++], t.head && L && t.length < 65536 && (t.head.comment += String.fromCharCode(L));
            while (L && d < s);
            if (t.flags & 512 && (t.check = Re(t.check, i, d, n)), s -= d, n += d, L)
              break e;
          } else t.head && (t.head.comment = null);
          t.mode = ea;
        case ea:
          if (t.flags & 512) {
            for (; l < 16; ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            if (f !== (t.check & 65535)) {
              e.msg = "header crc mismatch", t.mode = J;
              break;
            }
            f = 0, l = 0;
          }
          t.head && (t.head.hcrc = t.flags >> 9 & 1, t.head.done = !0), e.adler = t.check = 0, t.mode = Fe;
          break;
        case ta:
          for (; l < 32; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          e.adler = t.check = va(f), f = 0, l = 0, t.mode = kr;
        case kr:
          if (t.havedict === 0)
            return e.next_out = o, e.avail_out = v, e.next_in = n, e.avail_in = s, t.hold = f, t.bits = l, vl;
          e.adler = t.check = 1, t.mode = Fe;
        case Fe:
          if (r === dl || r === lr)
            break e;
        case di:
          if (t.last) {
            f >>>= l & 7, l -= l & 7, t.mode = vi;
            break;
          }
          for (; l < 3; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          switch (t.last = f & 1, f >>>= 1, l -= 1, f & 3) {
            case 0:
              t.mode = ra;
              break;
            case 1:
              if (kl(t), t.mode = ur, r === lr) {
                f >>>= 2, l -= 2;
                break e;
              }
              break;
            case 2:
              t.mode = na;
              break;
            case 3:
              e.msg = "invalid block type", t.mode = J;
          }
          f >>>= 2, l -= 2;
          break;
        case ra:
          for (f >>>= l & 7, l -= l & 7; l < 32; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          if ((f & 65535) !== (f >>> 16 ^ 65535)) {
            e.msg = "invalid stored block lengths", t.mode = J;
            break;
          }
          if (t.length = f & 65535, f = 0, l = 0, t.mode = ci, r === lr)
            break e;
        case ci:
          t.mode = ia;
        case ia:
          if (d = t.length, d) {
            if (d > s && (d = s), d > v && (d = v), d === 0)
              break e;
            ve.arraySet(a, i, n, d, o), s -= d, n += d, v -= d, o += d, t.length -= d;
            break;
          }
          t.mode = Fe;
          break;
        case na:
          for (; l < 14; ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          if (t.nlen = (f & 31) + 257, f >>>= 5, l -= 5, t.ndist = (f & 31) + 1, f >>>= 5, l -= 5, t.ncode = (f & 15) + 4, f >>>= 4, l -= 4, t.nlen > 286 || t.ndist > 30) {
            e.msg = "too many length or distance symbols", t.mode = J;
            break;
          }
          t.have = 0, t.mode = aa;
        case aa:
          for (; t.have < t.ncode; ) {
            for (; l < 3; ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            t.lens[X[t.have++]] = f & 7, f >>>= 3, l -= 3;
          }
          for (; t.have < 19; )
            t.lens[X[t.have++]] = 0;
          if (t.lencode = t.lendyn, t.lenbits = 7, M = { bits: t.lenbits }, B = zt(hl, t.lens, 0, 19, t.lencode, 0, t.work, M), t.lenbits = M.bits, B) {
            e.msg = "invalid code lengths set", t.mode = J;
            break;
          }
          t.have = 0, t.mode = oa;
        case oa:
          for (; t.have < t.nlen + t.ndist; ) {
            for (; C = t.lencode[f & (1 << t.lenbits) - 1], u = C >>> 24, _ = C >>> 16 & 255, y = C & 65535, !(u <= l); ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            if (y < 16)
              f >>>= u, l -= u, t.lens[t.have++] = y;
            else {
              if (y === 16) {
                for (D = u + 2; l < D; ) {
                  if (s === 0)
                    break e;
                  s--, f += i[n++] << l, l += 8;
                }
                if (f >>>= u, l -= u, t.have === 0) {
                  e.msg = "invalid bit length repeat", t.mode = J;
                  break;
                }
                L = t.lens[t.have - 1], d = 3 + (f & 3), f >>>= 2, l -= 2;
              } else if (y === 17) {
                for (D = u + 3; l < D; ) {
                  if (s === 0)
                    break e;
                  s--, f += i[n++] << l, l += 8;
                }
                f >>>= u, l -= u, L = 0, d = 3 + (f & 7), f >>>= 3, l -= 3;
              } else {
                for (D = u + 7; l < D; ) {
                  if (s === 0)
                    break e;
                  s--, f += i[n++] << l, l += 8;
                }
                f >>>= u, l -= u, L = 0, d = 11 + (f & 127), f >>>= 7, l -= 7;
              }
              if (t.have + d > t.nlen + t.ndist) {
                e.msg = "invalid bit length repeat", t.mode = J;
                break;
              }
              for (; d--; )
                t.lens[t.have++] = L;
            }
          }
          if (t.mode === J)
            break;
          if (t.lens[256] === 0) {
            e.msg = "invalid code -- missing end-of-block", t.mode = J;
            break;
          }
          if (t.lenbits = 9, M = { bits: t.lenbits }, B = zt(bo, t.lens, 0, t.nlen, t.lencode, 0, t.work, M), t.lenbits = M.bits, B) {
            e.msg = "invalid literal/lengths set", t.mode = J;
            break;
          }
          if (t.distbits = 6, t.distcode = t.distdyn, M = { bits: t.distbits }, B = zt(xo, t.lens, t.nlen, t.ndist, t.distcode, 0, t.work, M), t.distbits = M.bits, B) {
            e.msg = "invalid distances set", t.mode = J;
            break;
          }
          if (t.mode = ur, r === lr)
            break e;
        case ur:
          t.mode = hr;
        case hr:
          if (s >= 6 && v >= 258) {
            e.next_out = o, e.avail_out = v, e.next_in = n, e.avail_in = s, t.hold = f, t.bits = l, ul(e, h), o = e.next_out, a = e.output, v = e.avail_out, n = e.next_in, i = e.input, s = e.avail_in, f = t.hold, l = t.bits, t.mode === Fe && (t.back = -1);
            break;
          }
          for (t.back = 0; C = t.lencode[f & (1 << t.lenbits) - 1], u = C >>> 24, _ = C >>> 16 & 255, y = C & 65535, !(u <= l); ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          if (_ && !(_ & 240)) {
            for (E = u, S = _, F = y; C = t.lencode[F + ((f & (1 << E + S) - 1) >> E)], u = C >>> 24, _ = C >>> 16 & 255, y = C & 65535, !(E + u <= l); ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            f >>>= E, l -= E, t.back += E;
          }
          if (f >>>= u, l -= u, t.back += u, t.length = y, _ === 0) {
            t.mode = ha;
            break;
          }
          if (_ & 32) {
            t.back = -1, t.mode = Fe;
            break;
          }
          if (_ & 64) {
            e.msg = "invalid literal/length code", t.mode = J;
            break;
          }
          t.extra = _ & 15, t.mode = sa;
        case sa:
          if (t.extra) {
            for (D = t.extra; l < D; ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            t.length += f & (1 << t.extra) - 1, f >>>= t.extra, l -= t.extra, t.back += t.extra;
          }
          t.was = t.length, t.mode = fa;
        case fa:
          for (; C = t.distcode[f & (1 << t.distbits) - 1], u = C >>> 24, _ = C >>> 16 & 255, y = C & 65535, !(u <= l); ) {
            if (s === 0)
              break e;
            s--, f += i[n++] << l, l += 8;
          }
          if (!(_ & 240)) {
            for (E = u, S = _, F = y; C = t.distcode[F + ((f & (1 << E + S) - 1) >> E)], u = C >>> 24, _ = C >>> 16 & 255, y = C & 65535, !(E + u <= l); ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            f >>>= E, l -= E, t.back += E;
          }
          if (f >>>= u, l -= u, t.back += u, _ & 64) {
            e.msg = "invalid distance code", t.mode = J;
            break;
          }
          t.offset = y, t.extra = _ & 15, t.mode = la;
        case la:
          if (t.extra) {
            for (D = t.extra; l < D; ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            t.offset += f & (1 << t.extra) - 1, f >>>= t.extra, l -= t.extra, t.back += t.extra;
          }
          if (t.offset > t.dmax) {
            e.msg = "invalid distance too far back", t.mode = J;
            break;
          }
          t.mode = ua;
        case ua:
          if (v === 0)
            break e;
          if (d = h - v, t.offset > d) {
            if (d = t.offset - d, d > t.whave && t.sane) {
              e.msg = "invalid distance too far back", t.mode = J;
              break;
            }
            d > t.wnext ? (d -= t.wnext, g = t.wsize - d) : g = t.wnext - d, d > t.length && (d = t.length), m = t.window;
          } else
            m = a, g = o - t.offset, d = t.length;
          d > v && (d = v), v -= d, t.length -= d;
          do
            a[o++] = m[g++];
          while (--d);
          t.length === 0 && (t.mode = hr);
          break;
        case ha:
          if (v === 0)
            break e;
          a[o++] = t.length, v--, t.mode = hr;
          break;
        case vi:
          if (t.wrap) {
            for (; l < 32; ) {
              if (s === 0)
                break e;
              s--, f |= i[n++] << l, l += 8;
            }
            if (h -= v, e.total_out += h, t.total += h, h && (e.adler = t.check = /*UPDATE(state.check, put - _out, _out);*/
            t.flags ? Re(t.check, a, h, o - h) : Ai(t.check, a, h, o - h)), h = v, (t.flags ? f : va(f)) !== t.check) {
              e.msg = "incorrect data check", t.mode = J;
              break;
            }
            f = 0, l = 0;
          }
          t.mode = da;
        case da:
          if (t.wrap && t.flags) {
            for (; l < 32; ) {
              if (s === 0)
                break e;
              s--, f += i[n++] << l, l += 8;
            }
            if (f !== (t.total & 4294967295)) {
              e.msg = "incorrect length check", t.mode = J;
              break;
            }
            f = 0, l = 0;
          }
          t.mode = ca;
        case ca:
          B = cl;
          break e;
        case J:
          B = ko;
          break e;
        case Co:
          return Eo;
        case _l:
        default:
          return we;
      }
  return e.next_out = o, e.avail_out = v, e.next_in = n, e.avail_in = s, t.hold = f, t.bits = l, (t.wsize || h !== e.avail_out && t.mode < J && (t.mode < vi || r !== Hn)) && Do(e, e.output, e.next_out, h - e.avail_out), w -= e.avail_in, h -= e.avail_out, e.total_in += w, e.total_out += h, t.total += h, t.wrap && h && (e.adler = t.check = /*UPDATE(state.check, strm.next_out - _out, _out);*/
  t.flags ? Re(t.check, a, h, e.next_out - h) : Ai(t.check, a, h, e.next_out - h)), e.data_type = t.bits + (t.last ? 64 : 0) + (t.mode === Fe ? 128 : 0) + (t.mode === ur || t.mode === ci ? 256 : 0), (w === 0 && h === 0 || r === Hn) && B === nt && (B = pl), B;
}
function Sl(e) {
  if (!e || !e.state)
    return we;
  var r = e.state;
  return r.window && (r.window = null), e.state = null, nt;
}
function Cl(e, r) {
  var t;
  return !e || !e.state || (t = e.state, !(t.wrap & 2)) ? we : (t.head = r, r.done = !1, nt);
}
function Tl(e, r) {
  var t = r.length, i, a, n;
  return !e || !e.state || (i = e.state, i.wrap !== 0 && i.mode !== kr) ? we : i.mode === kr && (a = 1, a = Ai(a, r, t, 0), a !== i.check) ? ko : (n = Do(e, r, t, t), n ? (i.mode = Co, Eo) : (i.havedict = 1, nt));
}
Ee.inflateReset = Ao;
Ee.inflateReset2 = Ro;
Ee.inflateResetKeep = To;
Ee.inflateInit = xl;
Ee.inflateInit2 = Oo;
Ee.inflate = El;
Ee.inflateEnd = Sl;
Ee.inflateGetHeader = Cl;
Ee.inflateSetDictionary = Tl;
Ee.inflateInfo = "pako inflate (from Nodeca project)";
var Io = {
  /* Allowed flush values; see deflate() and inflate() below for details */
  Z_NO_FLUSH: 0,
  Z_PARTIAL_FLUSH: 1,
  Z_SYNC_FLUSH: 2,
  Z_FULL_FLUSH: 3,
  Z_FINISH: 4,
  Z_BLOCK: 5,
  Z_TREES: 6,
  /* Return codes for the compression/decompression functions. Negative values
  * are errors, positive values are used for special but normal events.
  */
  Z_OK: 0,
  Z_STREAM_END: 1,
  Z_NEED_DICT: 2,
  Z_ERRNO: -1,
  Z_STREAM_ERROR: -2,
  Z_DATA_ERROR: -3,
  //Z_MEM_ERROR:     -4,
  Z_BUF_ERROR: -5,
  //Z_VERSION_ERROR: -6,
  /* compression levels */
  Z_NO_COMPRESSION: 0,
  Z_BEST_SPEED: 1,
  Z_BEST_COMPRESSION: 9,
  Z_DEFAULT_COMPRESSION: -1,
  Z_FILTERED: 1,
  Z_HUFFMAN_ONLY: 2,
  Z_RLE: 3,
  Z_FIXED: 4,
  Z_DEFAULT_STRATEGY: 0,
  /* Possible values of the data_type field (though see inflate()) */
  Z_BINARY: 0,
  Z_TEXT: 1,
  //Z_ASCII:                1, // = Z_TEXT (deprecated)
  Z_UNKNOWN: 2,
  /* The deflate compression method */
  Z_DEFLATED: 8
  //Z_NULL:                 null // Use -1 or null inline, depending on var type
};
function Al() {
  this.text = 0, this.time = 0, this.xflags = 0, this.os = 0, this.extra = null, this.extra_len = 0, this.name = "", this.comment = "", this.hcrc = 0, this.done = !1;
}
var Rl = Al, wt = Ee, Pt = ze, wr = st, ee = Io, Ri = Wi, Ol = wo, Dl = Rl, Bo = Object.prototype.toString;
function at(e) {
  if (!(this instanceof at)) return new at(e);
  this.options = Pt.assign({
    chunkSize: 16384,
    windowBits: 0,
    to: ""
  }, e || {});
  var r = this.options;
  r.raw && r.windowBits >= 0 && r.windowBits < 16 && (r.windowBits = -r.windowBits, r.windowBits === 0 && (r.windowBits = -15)), r.windowBits >= 0 && r.windowBits < 16 && !(e && e.windowBits) && (r.windowBits += 32), r.windowBits > 15 && r.windowBits < 48 && (r.windowBits & 15 || (r.windowBits |= 15)), this.err = 0, this.msg = "", this.ended = !1, this.chunks = [], this.strm = new Ol(), this.strm.avail_out = 0;
  var t = wt.inflateInit2(
    this.strm,
    r.windowBits
  );
  if (t !== ee.Z_OK)
    throw new Error(Ri[t]);
  if (this.header = new Dl(), wt.inflateGetHeader(this.strm, this.header), r.dictionary && (typeof r.dictionary == "string" ? r.dictionary = wr.string2buf(r.dictionary) : Bo.call(r.dictionary) === "[object ArrayBuffer]" && (r.dictionary = new Uint8Array(r.dictionary)), r.raw && (t = wt.inflateSetDictionary(this.strm, r.dictionary), t !== ee.Z_OK)))
    throw new Error(Ri[t]);
}
at.prototype.push = function(e, r) {
  var t = this.strm, i = this.options.chunkSize, a = this.options.dictionary, n, o, s, v, f, l = !1;
  if (this.ended)
    return !1;
  o = r === ~~r ? r : r === !0 ? ee.Z_FINISH : ee.Z_NO_FLUSH, typeof e == "string" ? t.input = wr.binstring2buf(e) : Bo.call(e) === "[object ArrayBuffer]" ? t.input = new Uint8Array(e) : t.input = e, t.next_in = 0, t.avail_in = t.input.length;
  do {
    if (t.avail_out === 0 && (t.output = new Pt.Buf8(i), t.next_out = 0, t.avail_out = i), n = wt.inflate(t, ee.Z_NO_FLUSH), n === ee.Z_NEED_DICT && a && (n = wt.inflateSetDictionary(this.strm, a)), n === ee.Z_BUF_ERROR && l === !0 && (n = ee.Z_OK, l = !1), n !== ee.Z_STREAM_END && n !== ee.Z_OK)
      return this.onEnd(n), this.ended = !0, !1;
    t.next_out && (t.avail_out === 0 || n === ee.Z_STREAM_END || t.avail_in === 0 && (o === ee.Z_FINISH || o === ee.Z_SYNC_FLUSH)) && (this.options.to === "string" ? (s = wr.utf8border(t.output, t.next_out), v = t.next_out - s, f = wr.buf2string(t.output, s), t.next_out = v, t.avail_out = i - v, v && Pt.arraySet(t.output, t.output, s, v, 0), this.onData(f)) : this.onData(Pt.shrinkBuf(t.output, t.next_out))), t.avail_in === 0 && t.avail_out === 0 && (l = !0);
  } while ((t.avail_in > 0 || t.avail_out === 0) && n !== ee.Z_STREAM_END);
  return n === ee.Z_STREAM_END && (o = ee.Z_FINISH), o === ee.Z_FINISH ? (n = wt.inflateEnd(this.strm), this.onEnd(n), this.ended = !0, n === ee.Z_OK) : (o === ee.Z_SYNC_FLUSH && (this.onEnd(ee.Z_OK), t.avail_out = 0), !0);
};
at.prototype.onData = function(e) {
  this.chunks.push(e);
};
at.prototype.onEnd = function(e) {
  e === ee.Z_OK && (this.options.to === "string" ? this.result = this.chunks.join("") : this.result = Pt.flattenChunks(this.chunks)), this.chunks = [], this.err = e, this.msg = this.strm.msg;
};
function qi(e, r) {
  var t = new at(r);
  if (t.push(e, !0), t.err)
    throw t.msg || Ri[t.err];
  return t.result;
}
function Il(e, r) {
  return r = r || {}, r.raw = !0, qi(e, r);
}
Jt.Inflate = at;
Jt.inflate = qi;
Jt.inflateRaw = Il;
Jt.ungzip = qi;
var Bl = ze.assign, Fl = Gt, Ll = Jt, Nl = Io, Fo = {};
Bl(Fo, Fl, Ll, Nl);
var zl = Fo, Pl = typeof Uint8Array < "u" && typeof Uint16Array < "u" && typeof Uint32Array < "u", jl = zl, Lo = Q(), zr = ye, Ul = Pl ? "uint8array" : "array";
Br.magic = "\b\0";
function ft(e, r) {
  zr.call(this, "FlateWorker/" + e), this._pako = null, this._pakoAction = e, this._pakoOptions = r, this.meta = {};
}
Lo.inherits(ft, zr);
ft.prototype.processChunk = function(e) {
  this.meta = e.meta, this._pako === null && this._createPako(), this._pako.push(Lo.transformTo(Ul, e.data), !1);
};
ft.prototype.flush = function() {
  zr.prototype.flush.call(this), this._pako === null && this._createPako(), this._pako.push([], !0);
};
ft.prototype.cleanUp = function() {
  zr.prototype.cleanUp.call(this), this._pako = null;
};
ft.prototype._createPako = function() {
  this._pako = new jl[this._pakoAction]({
    raw: !0,
    level: this._pakoOptions.level || -1
    // default compression
  });
  var e = this;
  this._pako.onData = function(r) {
    e.push({
      data: r,
      meta: e.meta
    });
  };
};
Br.compressWorker = function(e) {
  return new ft("Deflate", e);
};
Br.uncompressWorker = function() {
  return new ft("Inflate", {});
};
var _a = ye;
Ir.STORE = {
  magic: "\0\0",
  compressWorker: function() {
    return new _a("STORE compression");
  },
  uncompressWorker: function() {
    return new _a("STORE decompression");
  }
};
Ir.DEFLATE = Br;
var Ke = {};
Ke.LOCAL_FILE_HEADER = "PK";
Ke.CENTRAL_FILE_HEADER = "PK";
Ke.CENTRAL_DIRECTORY_END = "PK";
Ke.ZIP64_CENTRAL_DIRECTORY_LOCATOR = "PK\x07";
Ke.ZIP64_CENTRAL_DIRECTORY_END = "PK";
Ke.DATA_DESCRIPTOR = "PK\x07\b";
var _t = Q(), Tt = ye, gi = xt, ga = Bi, Er = Ke, G = function(e, r) {
  var t = "", i;
  for (i = 0; i < r; i++)
    t += String.fromCharCode(e & 255), e = e >>> 8;
  return t;
}, Ml = function(e, r) {
  var t = e;
  return e || (t = r ? 16893 : 33204), (t & 65535) << 16;
}, $l = function(e) {
  return (e || 0) & 63;
}, No = function(e, r, t, i, a, n) {
  var o = e.file, s = e.compression, v = n !== gi.utf8encode, f = _t.transformTo("string", n(o.name)), l = _t.transformTo("string", gi.utf8encode(o.name)), w = o.comment, h = _t.transformTo("string", n(w)), d = _t.transformTo("string", gi.utf8encode(w)), g = l.length !== o.name.length, m = d.length !== w.length, C, u, _ = "", y = "", E = "", S = o.dir, F = o.date, L = {
    crc32: 0,
    compressedSize: 0,
    uncompressedSize: 0
  };
  (!r || t) && (L.crc32 = e.crc32, L.compressedSize = e.compressedSize, L.uncompressedSize = e.uncompressedSize);
  var B = 0;
  r && (B |= 8), !v && (g || m) && (B |= 2048);
  var z = 0, M = 0;
  S && (z |= 16), a === "UNIX" ? (M = 798, z |= Ml(o.unixPermissions, S)) : (M = 20, z |= $l(o.dosPermissions)), C = F.getUTCHours(), C = C << 6, C = C | F.getUTCMinutes(), C = C << 5, C = C | F.getUTCSeconds() / 2, u = F.getUTCFullYear() - 1980, u = u << 4, u = u | F.getUTCMonth() + 1, u = u << 5, u = u | F.getUTCDate(), g && (y = // Version
  G(1, 1) + // NameCRC32
  G(ga(f), 4) + // UnicodeName
  l, _ += // Info-ZIP Unicode Path Extra Field
  "up" + // size
  G(y.length, 2) + // content
  y), m && (E = // Version
  G(1, 1) + // CommentCRC32
  G(ga(h), 4) + // UnicodeName
  d, _ += // Info-ZIP Unicode Path Extra Field
  "uc" + // size
  G(E.length, 2) + // content
  E);
  var D = "";
  D += `
\0`, D += G(B, 2), D += s.magic, D += G(C, 2), D += G(u, 2), D += G(L.crc32, 4), D += G(L.compressedSize, 4), D += G(L.uncompressedSize, 4), D += G(f.length, 2), D += G(_.length, 2);
  var X = Er.LOCAL_FILE_HEADER + D + f + _, se = Er.CENTRAL_FILE_HEADER + // version made by (00: DOS)
  G(M, 2) + // file header (common to file and central directory)
  D + // file comment length
  G(h.length, 2) + // disk number start
  "\0\0\0\0" + // external file attributes
  G(z, 4) + // relative offset of local header
  G(i, 4) + // file name
  f + // extra field
  _ + // file comment
  h;
  return {
    fileRecord: X,
    dirRecord: se
  };
}, Zl = function(e, r, t, i, a) {
  var n = "", o = _t.transformTo("string", a(i));
  return n = Er.CENTRAL_DIRECTORY_END + // number of this disk
  "\0\0\0\0" + // total number of entries in the central directory on this disk
  G(e, 2) + // total number of entries in the central directory
  G(e, 2) + // size of the central directory   4 bytes
  G(r, 4) + // offset of start of central directory with respect to the starting disk number
  G(t, 4) + // .ZIP file comment length
  G(o.length, 2) + // .ZIP file comment
  o, n;
}, Wl = function(e) {
  var r = "";
  return r = Er.DATA_DESCRIPTOR + // crc-32                          4 bytes
  G(e.crc32, 4) + // compressed size                 4 bytes
  G(e.compressedSize, 4) + // uncompressed size               4 bytes
  G(e.uncompressedSize, 4), r;
};
function Se(e, r, t, i) {
  Tt.call(this, "ZipFileWorker"), this.bytesWritten = 0, this.zipComment = r, this.zipPlatform = t, this.encodeFileName = i, this.streamFiles = e, this.accumulate = !1, this.contentBuffer = [], this.dirRecords = [], this.currentSourceOffset = 0, this.entriesCount = 0, this.currentFile = null, this._sources = [];
}
_t.inherits(Se, Tt);
Se.prototype.push = function(e) {
  var r = e.meta.percent || 0, t = this.entriesCount, i = this._sources.length;
  this.accumulate ? this.contentBuffer.push(e) : (this.bytesWritten += e.data.length, Tt.prototype.push.call(this, {
    data: e.data,
    meta: {
      currentFile: this.currentFile,
      percent: t ? (r + 100 * (t - i - 1)) / t : 100
    }
  }));
};
Se.prototype.openedSource = function(e) {
  this.currentSourceOffset = this.bytesWritten, this.currentFile = e.file.name;
  var r = this.streamFiles && !e.file.dir;
  if (r) {
    var t = No(e, r, !1, this.currentSourceOffset, this.zipPlatform, this.encodeFileName);
    this.push({
      data: t.fileRecord,
      meta: { percent: 0 }
    });
  } else
    this.accumulate = !0;
};
Se.prototype.closedSource = function(e) {
  this.accumulate = !1;
  var r = this.streamFiles && !e.file.dir, t = No(e, r, !0, this.currentSourceOffset, this.zipPlatform, this.encodeFileName);
  if (this.dirRecords.push(t.dirRecord), r)
    this.push({
      data: Wl(e),
      meta: { percent: 100 }
    });
  else
    for (this.push({
      data: t.fileRecord,
      meta: { percent: 0 }
    }); this.contentBuffer.length; )
      this.push(this.contentBuffer.shift());
  this.currentFile = null;
};
Se.prototype.flush = function() {
  for (var e = this.bytesWritten, r = 0; r < this.dirRecords.length; r++)
    this.push({
      data: this.dirRecords[r],
      meta: { percent: 100 }
    });
  var t = this.bytesWritten - e, i = Zl(this.dirRecords.length, t, e, this.zipComment, this.encodeFileName);
  this.push({
    data: i,
    meta: { percent: 100 }
  });
};
Se.prototype.prepareNextSource = function() {
  this.previous = this._sources.shift(), this.openedSource(this.previous.streamInfo), this.isPaused ? this.previous.pause() : this.previous.resume();
};
Se.prototype.registerPrevious = function(e) {
  this._sources.push(e);
  var r = this;
  return e.on("data", function(t) {
    r.processChunk(t);
  }), e.on("end", function() {
    r.closedSource(r.previous.streamInfo), r._sources.length ? r.prepareNextSource() : r.end();
  }), e.on("error", function(t) {
    r.error(t);
  }), this;
};
Se.prototype.resume = function() {
  if (!Tt.prototype.resume.call(this))
    return !1;
  if (!this.previous && this._sources.length)
    return this.prepareNextSource(), !0;
  if (!this.previous && !this._sources.length && !this.generatedError)
    return this.end(), !0;
};
Se.prototype.error = function(e) {
  var r = this._sources;
  if (!Tt.prototype.error.call(this, e))
    return !1;
  for (var t = 0; t < r.length; t++)
    try {
      r[t].error(e);
    } catch {
    }
  return !0;
};
Se.prototype.lock = function() {
  Tt.prototype.lock.call(this);
  for (var e = this._sources, r = 0; r < e.length; r++)
    e[r].lock();
};
var Hl = Se, ql = Ir, Yl = Hl, Kl = function(e, r) {
  var t = e || r, i = ql[t];
  if (!i)
    throw new Error(t + " is not a valid compression method !");
  return i;
};
qa.generateWorker = function(e, r, t) {
  var i = new Yl(r.streamFiles, t, r.platform, r.encodeFileName), a = 0;
  try {
    e.forEach(function(n, o) {
      a++;
      var s = Kl(o.options.compression, r.compression), v = o.options.compressionOptions || r.compressionOptions || {}, f = o.dir, l = o.date;
      o._compressWorker(s, v).withStreamInfo("file", {
        name: n,
        dir: f,
        date: l,
        comment: o.comment || "",
        unixPermissions: o.unixPermissions,
        dosPermissions: o.dosPermissions
      }).pipe(i);
    }), i.entriesCount = a;
  } catch (n) {
    i.error(n);
  }
  return i;
};
var Gl = Q(), Pr = ye;
function Qt(e, r) {
  Pr.call(this, "Nodejs stream input adapter for " + e), this._upstreamEnded = !1, this._bindStream(r);
}
Gl.inherits(Qt, Pr);
Qt.prototype._bindStream = function(e) {
  var r = this;
  this._stream = e, e.pause(), e.on("data", function(t) {
    r.push({
      data: t,
      meta: {
        percent: 0
      }
    });
  }).on("error", function(t) {
    r.isPaused ? this.generatedError = t : r.error(t);
  }).on("end", function() {
    r.isPaused ? r._upstreamEnded = !0 : r.end();
  });
};
Qt.prototype.pause = function() {
  return Pr.prototype.pause.call(this) ? (this._stream.pause(), !0) : !1;
};
Qt.prototype.resume = function() {
  return Pr.prototype.resume.call(this) ? (this._upstreamEnded ? this.end() : this._stream.resume(), !0) : !1;
};
var Vl = Qt, Xl = xt, jt = Q(), zo = ye, Jl = Ma, Po = be, ma = Pi, Ql = Ws, eu = qa, wa = Rr, tu = Vl, jo = function(e, r, t) {
  var i = jt.getTypeOf(r), a, n = jt.extend(t || {}, Po);
  n.date = n.date || /* @__PURE__ */ new Date(), n.compression !== null && (n.compression = n.compression.toUpperCase()), typeof n.unixPermissions == "string" && (n.unixPermissions = parseInt(n.unixPermissions, 8)), n.unixPermissions && n.unixPermissions & 16384 && (n.dir = !0), n.dosPermissions && n.dosPermissions & 16 && (n.dir = !0), n.dir && (e = Uo(e)), n.createFolders && (a = ru(e)) && Mo.call(this, a, !0);
  var o = i === "string" && n.binary === !1 && n.base64 === !1;
  (!t || typeof t.binary > "u") && (n.binary = !o);
  var s = r instanceof ma && r.uncompressedSize === 0;
  (s || n.dir || !r || r.length === 0) && (n.base64 = !1, n.binary = !0, r = "", n.compression = "STORE", i = "string");
  var v = null;
  r instanceof ma || r instanceof zo ? v = r : wa.isNode && wa.isStream(r) ? v = new tu(e, r) : v = jt.prepareContent(e, r, n.binary, n.optimizedBinaryString, n.base64);
  var f = new Ql(e, v, n);
  this.files[e] = f;
}, ru = function(e) {
  e.slice(-1) === "/" && (e = e.substring(0, e.length - 1));
  var r = e.lastIndexOf("/");
  return r > 0 ? e.substring(0, r) : "";
}, Uo = function(e) {
  return e.slice(-1) !== "/" && (e += "/"), e;
}, Mo = function(e, r) {
  return r = typeof r < "u" ? r : Po.createFolders, e = Uo(e), this.files[e] || jo.call(this, e, null, {
    dir: !0,
    createFolders: r
  }), this.files[e];
};
function ya(e) {
  return Object.prototype.toString.call(e) === "[object RegExp]";
}
var iu = {
  /**
   * @see loadAsync
   */
  load: function() {
    throw new Error("This method has been removed in JSZip 3.0, please check the upgrade guide.");
  },
  /**
   * Call a callback function for each entry at this folder level.
   * @param {Function} cb the callback function:
   * function (relativePath, file) {...}
   * It takes 2 arguments : the relative path and the file.
   */
  forEach: function(e) {
    var r, t, i;
    for (r in this.files)
      i = this.files[r], t = r.slice(this.root.length, r.length), t && r.slice(0, this.root.length) === this.root && e(t, i);
  },
  /**
   * Filter nested files/folders with the specified function.
   * @param {Function} search the predicate to use :
   * function (relativePath, file) {...}
   * It takes 2 arguments : the relative path and the file.
   * @return {Array} An array of matching elements.
   */
  filter: function(e) {
    var r = [];
    return this.forEach(function(t, i) {
      e(t, i) && r.push(i);
    }), r;
  },
  /**
   * Add a file to the zip file, or search a file.
   * @param   {string|RegExp} name The name of the file to add (if data is defined),
   * the name of the file to find (if no data) or a regex to match files.
   * @param   {String|ArrayBuffer|Uint8Array|Buffer} data  The file data, either raw or base64 encoded
   * @param   {Object} o     File options
   * @return  {JSZip|Object|Array} this JSZip object (when adding a file),
   * a file (when searching by string) or an array of files (when searching by regex).
   */
  file: function(e, r, t) {
    if (arguments.length === 1)
      if (ya(e)) {
        var i = e;
        return this.filter(function(n, o) {
          return !o.dir && i.test(n);
        });
      } else {
        var a = this.files[this.root + e];
        return a && !a.dir ? a : null;
      }
    else
      e = this.root + e, jo.call(this, e, r, t);
    return this;
  },
  /**
   * Add a directory to the zip file, or search.
   * @param   {String|RegExp} arg The name of the directory to add, or a regex to search folders.
   * @return  {JSZip} an object with the new directory as the root, or an array containing matching folders.
   */
  folder: function(e) {
    if (!e)
      return this;
    if (ya(e))
      return this.filter(function(a, n) {
        return n.dir && e.test(a);
      });
    var r = this.root + e, t = Mo.call(this, r), i = this.clone();
    return i.root = t.name, i;
  },
  /**
   * Delete a file, or a directory and all sub-files, from the zip
   * @param {string} name the name of the file to delete
   * @return {JSZip} this JSZip object
   */
  remove: function(e) {
    e = this.root + e;
    var r = this.files[e];
    if (r || (e.slice(-1) !== "/" && (e += "/"), r = this.files[e]), r && !r.dir)
      delete this.files[e];
    else
      for (var t = this.filter(function(a, n) {
        return n.name.slice(0, e.length) === e;
      }), i = 0; i < t.length; i++)
        delete this.files[t[i].name];
    return this;
  },
  /**
   * @deprecated This method has been removed in JSZip 3.0, please check the upgrade guide.
   */
  generate: function() {
    throw new Error("This method has been removed in JSZip 3.0, please check the upgrade guide.");
  },
  /**
   * Generate the complete zip file as an internal stream.
   * @param {Object} options the options to generate the zip file :
   * - compression, "STORE" by default.
   * - type, "base64" by default. Values are : string, base64, uint8array, arraybuffer, blob.
   * @return {StreamHelper} the streamed zip file.
   */
  generateInternalStream: function(e) {
    var r, t = {};
    try {
      if (t = jt.extend(e || {}, {
        streamFiles: !1,
        compression: "STORE",
        compressionOptions: null,
        type: "",
        platform: "DOS",
        comment: null,
        mimeType: "application/zip",
        encodeFileName: Xl.utf8encode
      }), t.type = t.type.toLowerCase(), t.compression = t.compression.toUpperCase(), t.type === "binarystring" && (t.type = "string"), !t.type)
        throw new Error("No output type specified.");
      jt.checkSupport(t.type), (t.platform === "darwin" || t.platform === "freebsd" || t.platform === "linux" || t.platform === "sunos") && (t.platform = "UNIX"), t.platform === "win32" && (t.platform = "DOS");
      var i = t.comment || this.comment || "";
      r = eu.generateWorker(this, t, i);
    } catch (a) {
      r = new zo("error"), r.error(a);
    }
    return new Jl(r, t.type || "string", t.mimeType);
  },
  /**
   * Generate the complete zip file asynchronously.
   * @see generateInternalStream
   */
  generateAsync: function(e, r) {
    return this.generateInternalStream(e).accumulate(r);
  },
  /**
   * Generate the complete zip file asynchronously.
   * @see generateInternalStream
   */
  generateNodeStream: function(e, r) {
    return e = e || {}, e.type || (e.type = "nodebuffer"), this.generateInternalStream(e).toNodejsStream(r);
  }
}, nu = iu, au = Q();
function $o(e) {
  this.data = e, this.length = e.length, this.index = 0, this.zero = 0;
}
$o.prototype = {
  /**
   * Check that the offset will not go too far.
   * @param {string} offset the additional offset to check.
   * @throws {Error} an Error if the offset is out of bounds.
   */
  checkOffset: function(e) {
    this.checkIndex(this.index + e);
  },
  /**
   * Check that the specified index will not be too far.
   * @param {string} newIndex the index to check.
   * @throws {Error} an Error if the index is out of bounds.
   */
  checkIndex: function(e) {
    if (this.length < this.zero + e || e < 0)
      throw new Error("End of data reached (data length = " + this.length + ", asked index = " + e + "). Corrupted zip ?");
  },
  /**
   * Change the index.
   * @param {number} newIndex The new index.
   * @throws {Error} if the new index is out of the data.
   */
  setIndex: function(e) {
    this.checkIndex(e), this.index = e;
  },
  /**
   * Skip the next n bytes.
   * @param {number} n the number of bytes to skip.
   * @throws {Error} if the new index is out of the data.
   */
  skip: function(e) {
    this.setIndex(this.index + e);
  },
  /**
   * Get the byte at the specified index.
   * @param {number} i the index to use.
   * @return {number} a byte.
   */
  byteAt: function() {
  },
  /**
   * Get the next number with a given byte size.
   * @param {number} size the number of bytes to read.
   * @return {number} the corresponding number.
   */
  readInt: function(e) {
    var r = 0, t;
    for (this.checkOffset(e), t = this.index + e - 1; t >= this.index; t--)
      r = (r << 8) + this.byteAt(t);
    return this.index += e, r;
  },
  /**
   * Get the next string with a given byte size.
   * @param {number} size the number of bytes to read.
   * @return {string} the corresponding string.
   */
  readString: function(e) {
    return au.transformTo("string", this.readData(e));
  },
  /**
   * Get raw data without conversion, <size> bytes.
   * @param {number} size the number of bytes to read.
   * @return {Object} the raw data, implementation specific.
   */
  readData: function() {
  },
  /**
   * Find the last occurrence of a zip signature (4 bytes).
   * @param {string} sig the signature to find.
   * @return {number} the index of the last occurrence, -1 if not found.
   */
  lastIndexOfSignature: function() {
  },
  /**
   * Read the signature (4 bytes) at the current position and compare it with sig.
   * @param {string} sig the expected signature
   * @return {boolean} true if the signature matches, false otherwise.
   */
  readAndCheckSignature: function() {
  },
  /**
   * Get the next date.
   * @return {Date} the date.
   */
  readDate: function() {
    var e = this.readInt(4);
    return new Date(Date.UTC(
      (e >> 25 & 127) + 1980,
      // year
      (e >> 21 & 15) - 1,
      // month
      e >> 16 & 31,
      // day
      e >> 11 & 31,
      // hour
      e >> 5 & 63,
      // minute
      (e & 31) << 1
    ));
  }
};
var Zo = $o, Wo = Zo, ou = Q();
function At(e) {
  Wo.call(this, e);
  for (var r = 0; r < this.data.length; r++)
    e[r] = e[r] & 255;
}
ou.inherits(At, Wo);
At.prototype.byteAt = function(e) {
  return this.data[this.zero + e];
};
At.prototype.lastIndexOfSignature = function(e) {
  for (var r = e.charCodeAt(0), t = e.charCodeAt(1), i = e.charCodeAt(2), a = e.charCodeAt(3), n = this.length - 4; n >= 0; --n)
    if (this.data[n] === r && this.data[n + 1] === t && this.data[n + 2] === i && this.data[n + 3] === a)
      return n - this.zero;
  return -1;
};
At.prototype.readAndCheckSignature = function(e) {
  var r = e.charCodeAt(0), t = e.charCodeAt(1), i = e.charCodeAt(2), a = e.charCodeAt(3), n = this.readData(4);
  return r === n[0] && t === n[1] && i === n[2] && a === n[3];
};
At.prototype.readData = function(e) {
  if (this.checkOffset(e), e === 0)
    return [];
  var r = this.data.slice(this.zero + this.index, this.zero + this.index + e);
  return this.index += e, r;
};
var Ho = At, qo = Zo, su = Q();
function Rt(e) {
  qo.call(this, e);
}
su.inherits(Rt, qo);
Rt.prototype.byteAt = function(e) {
  return this.data.charCodeAt(this.zero + e);
};
Rt.prototype.lastIndexOfSignature = function(e) {
  return this.data.lastIndexOf(e) - this.zero;
};
Rt.prototype.readAndCheckSignature = function(e) {
  var r = this.readData(4);
  return e === r;
};
Rt.prototype.readData = function(e) {
  this.checkOffset(e);
  var r = this.data.slice(this.zero + this.index, this.zero + this.index + e);
  return this.index += e, r;
};
var fu = Rt, Yo = Ho, lu = Q();
function Yi(e) {
  Yo.call(this, e);
}
lu.inherits(Yi, Yo);
Yi.prototype.readData = function(e) {
  if (this.checkOffset(e), e === 0)
    return new Uint8Array(0);
  var r = this.data.subarray(this.zero + this.index, this.zero + this.index + e);
  return this.index += e, r;
};
var Ko = Yi, Go = Ko, uu = Q();
function Ki(e) {
  Go.call(this, e);
}
uu.inherits(Ki, Go);
Ki.prototype.readData = function(e) {
  this.checkOffset(e);
  var r = this.data.slice(this.zero + this.index, this.zero + this.index + e);
  return this.index += e, r;
};
var hu = Ki, dr = Q(), ba = ie, du = Ho, cu = fu, vu = hu, pu = Ko, Vo = function(e) {
  var r = dr.getTypeOf(e);
  return dr.checkSupport(r), r === "string" && !ba.uint8array ? new cu(e) : r === "nodebuffer" ? new vu(e) : ba.uint8array ? new pu(dr.transformTo("uint8array", e)) : new du(dr.transformTo("array", e));
}, mi = Vo, Ue = Q(), _u = Pi, xa = Bi, cr = xt, vr = Ir, gu = ie, mu = 0, wu = 3, yu = function(e) {
  for (var r in vr)
    if (Object.prototype.hasOwnProperty.call(vr, r) && vr[r].magic === e)
      return vr[r];
  return null;
};
function Xo(e, r) {
  this.options = e, this.loadOptions = r;
}
Xo.prototype = {
  /**
   * say if the file is encrypted.
   * @return {boolean} true if the file is encrypted, false otherwise.
   */
  isEncrypted: function() {
    return (this.bitFlag & 1) === 1;
  },
  /**
   * say if the file has utf-8 filename/comment.
   * @return {boolean} true if the filename/comment is in utf-8, false otherwise.
   */
  useUTF8: function() {
    return (this.bitFlag & 2048) === 2048;
  },
  /**
   * Read the local part of a zip file and add the info in this object.
   * @param {DataReader} reader the reader to use.
   */
  readLocalPart: function(e) {
    var r, t;
    if (e.skip(22), this.fileNameLength = e.readInt(2), t = e.readInt(2), this.fileName = e.readData(this.fileNameLength), e.skip(t), this.compressedSize === -1 || this.uncompressedSize === -1)
      throw new Error("Bug or corrupted zip : didn't get enough information from the central directory (compressedSize === -1 || uncompressedSize === -1)");
    if (r = yu(this.compressionMethod), r === null)
      throw new Error("Corrupted zip : compression " + Ue.pretty(this.compressionMethod) + " unknown (inner file : " + Ue.transformTo("string", this.fileName) + ")");
    this.decompressed = new _u(this.compressedSize, this.uncompressedSize, this.crc32, r, e.readData(this.compressedSize));
  },
  /**
   * Read the central part of a zip file and add the info in this object.
   * @param {DataReader} reader the reader to use.
   */
  readCentralPart: function(e) {
    this.versionMadeBy = e.readInt(2), e.skip(2), this.bitFlag = e.readInt(2), this.compressionMethod = e.readString(2), this.date = e.readDate(), this.crc32 = e.readInt(4), this.compressedSize = e.readInt(4), this.uncompressedSize = e.readInt(4);
    var r = e.readInt(2);
    if (this.extraFieldsLength = e.readInt(2), this.fileCommentLength = e.readInt(2), this.diskNumberStart = e.readInt(2), this.internalFileAttributes = e.readInt(2), this.externalFileAttributes = e.readInt(4), this.localHeaderOffset = e.readInt(4), this.isEncrypted())
      throw new Error("Encrypted zip are not supported");
    e.skip(r), this.readExtraFields(e), this.parseZIP64ExtraField(e), this.fileComment = e.readData(this.fileCommentLength);
  },
  /**
   * Parse the external file attributes and get the unix/dos permissions.
   */
  processAttributes: function() {
    this.unixPermissions = null, this.dosPermissions = null;
    var e = this.versionMadeBy >> 8;
    this.dir = !!(this.externalFileAttributes & 16), e === mu && (this.dosPermissions = this.externalFileAttributes & 63), e === wu && (this.unixPermissions = this.externalFileAttributes >> 16 & 65535), !this.dir && this.fileNameStr.slice(-1) === "/" && (this.dir = !0);
  },
  /**
   * Parse the ZIP64 extra field and merge the info in the current ZipEntry.
   * @param {DataReader} reader the reader to use.
   */
  parseZIP64ExtraField: function() {
    if (this.extraFields[1]) {
      var e = mi(this.extraFields[1].value);
      this.uncompressedSize === Ue.MAX_VALUE_32BITS && (this.uncompressedSize = e.readInt(8)), this.compressedSize === Ue.MAX_VALUE_32BITS && (this.compressedSize = e.readInt(8)), this.localHeaderOffset === Ue.MAX_VALUE_32BITS && (this.localHeaderOffset = e.readInt(8)), this.diskNumberStart === Ue.MAX_VALUE_32BITS && (this.diskNumberStart = e.readInt(4));
    }
  },
  /**
   * Read the central part of a zip file and add the info in this object.
   * @param {DataReader} reader the reader to use.
   */
  readExtraFields: function(e) {
    var r = e.index + this.extraFieldsLength, t, i, a;
    for (this.extraFields || (this.extraFields = {}); e.index + 4 < r; )
      t = e.readInt(2), i = e.readInt(2), a = e.readData(i), this.extraFields[t] = {
        id: t,
        length: i,
        value: a
      };
    e.setIndex(r);
  },
  /**
   * Apply an UTF8 transformation if needed.
   */
  handleUTF8: function() {
    var e = gu.uint8array ? "uint8array" : "array";
    if (this.useUTF8())
      this.fileNameStr = cr.utf8decode(this.fileName), this.fileCommentStr = cr.utf8decode(this.fileComment);
    else {
      var r = this.findExtraFieldUnicodePath();
      if (r !== null)
        this.fileNameStr = r;
      else {
        var t = Ue.transformTo(e, this.fileName);
        this.fileNameStr = this.loadOptions.decodeFileName(t);
      }
      var i = this.findExtraFieldUnicodeComment();
      if (i !== null)
        this.fileCommentStr = i;
      else {
        var a = Ue.transformTo(e, this.fileComment);
        this.fileCommentStr = this.loadOptions.decodeFileName(a);
      }
    }
  },
  /**
   * Find the unicode path declared in the extra field, if any.
   * @return {String} the unicode path, null otherwise.
   */
  findExtraFieldUnicodePath: function() {
    var e = this.extraFields[28789];
    if (e) {
      var r = mi(e.value);
      return r.readInt(1) !== 1 || xa(this.fileName) !== r.readInt(4) ? null : cr.utf8decode(r.readData(e.length - 5));
    }
    return null;
  },
  /**
   * Find the unicode comment declared in the extra field, if any.
   * @return {String} the unicode comment, null otherwise.
   */
  findExtraFieldUnicodeComment: function() {
    var e = this.extraFields[25461];
    if (e) {
      var r = mi(e.value);
      return r.readInt(1) !== 1 || xa(this.fileComment) !== r.readInt(4) ? null : cr.utf8decode(r.readData(e.length - 5));
    }
    return null;
  }
};
var bu = Xo, xu = Vo, Le = Q(), xe = Ke, ku = bu, Eu = ie;
function Jo(e) {
  this.files = [], this.loadOptions = e;
}
Jo.prototype = {
  /**
   * Check that the reader is on the specified signature.
   * @param {string} expectedSignature the expected signature.
   * @throws {Error} if it is an other signature.
   */
  checkSignature: function(e) {
    if (!this.reader.readAndCheckSignature(e)) {
      this.reader.index -= 4;
      var r = this.reader.readString(4);
      throw new Error("Corrupted zip or bug: unexpected signature (" + Le.pretty(r) + ", expected " + Le.pretty(e) + ")");
    }
  },
  /**
   * Check if the given signature is at the given index.
   * @param {number} askedIndex the index to check.
   * @param {string} expectedSignature the signature to expect.
   * @return {boolean} true if the signature is here, false otherwise.
   */
  isSignature: function(e, r) {
    var t = this.reader.index;
    this.reader.setIndex(e);
    var i = this.reader.readString(4), a = i === r;
    return this.reader.setIndex(t), a;
  },
  /**
   * Read the end of the central directory.
   */
  readBlockEndOfCentral: function() {
    this.diskNumber = this.reader.readInt(2), this.diskWithCentralDirStart = this.reader.readInt(2), this.centralDirRecordsOnThisDisk = this.reader.readInt(2), this.centralDirRecords = this.reader.readInt(2), this.centralDirSize = this.reader.readInt(4), this.centralDirOffset = this.reader.readInt(4), this.zipCommentLength = this.reader.readInt(2);
    var e = this.reader.readData(this.zipCommentLength), r = Eu.uint8array ? "uint8array" : "array", t = Le.transformTo(r, e);
    this.zipComment = this.loadOptions.decodeFileName(t);
  },
  /**
   * Read the end of the Zip 64 central directory.
   * Not merged with the method readEndOfCentral :
   * The end of central can coexist with its Zip64 brother,
   * I don't want to read the wrong number of bytes !
   */
  readBlockZip64EndOfCentral: function() {
    this.zip64EndOfCentralSize = this.reader.readInt(8), this.reader.skip(4), this.diskNumber = this.reader.readInt(4), this.diskWithCentralDirStart = this.reader.readInt(4), this.centralDirRecordsOnThisDisk = this.reader.readInt(8), this.centralDirRecords = this.reader.readInt(8), this.centralDirSize = this.reader.readInt(8), this.centralDirOffset = this.reader.readInt(8), this.zip64ExtensibleData = {};
    for (var e = this.zip64EndOfCentralSize - 44, r = 0, t, i, a; r < e; )
      t = this.reader.readInt(2), i = this.reader.readInt(4), a = this.reader.readData(i), this.zip64ExtensibleData[t] = {
        id: t,
        length: i,
        value: a
      };
  },
  /**
   * Read the end of the Zip 64 central directory locator.
   */
  readBlockZip64EndOfCentralLocator: function() {
    if (this.diskWithZip64CentralDirStart = this.reader.readInt(4), this.relativeOffsetEndOfZip64CentralDir = this.reader.readInt(8), this.disksCount = this.reader.readInt(4), this.disksCount > 1)
      throw new Error("Multi-volumes zip are not supported");
  },
  /**
   * Read the local files, based on the offset read in the central part.
   */
  readLocalFiles: function() {
    var e, r;
    for (e = 0; e < this.files.length; e++)
      r = this.files[e], this.reader.setIndex(r.localHeaderOffset), this.checkSignature(xe.LOCAL_FILE_HEADER), r.readLocalPart(this.reader), r.handleUTF8(), r.processAttributes();
  },
  /**
   * Read the central directory.
   */
  readCentralDir: function() {
    var e;
    for (this.reader.setIndex(this.centralDirOffset); this.reader.readAndCheckSignature(xe.CENTRAL_FILE_HEADER); )
      e = new ku({
        zip64: this.zip64
      }, this.loadOptions), e.readCentralPart(this.reader), this.files.push(e);
    if (this.centralDirRecords !== this.files.length && this.centralDirRecords !== 0 && this.files.length === 0)
      throw new Error("Corrupted zip or bug: expected " + this.centralDirRecords + " records in central dir, got " + this.files.length);
  },
  /**
   * Read the end of central directory.
   */
  readEndOfCentral: function() {
    var e = this.reader.lastIndexOfSignature(xe.CENTRAL_DIRECTORY_END);
    if (e < 0) {
      var r = !this.isSignature(0, xe.LOCAL_FILE_HEADER);
      throw r ? new Error("Can't find end of central directory : is this a zip file ? If it is, see https://stuk.github.io/jszip/documentation/howto/read_zip.html") : new Error("Corrupted zip: can't find end of central directory");
    }
    this.reader.setIndex(e);
    var t = e;
    if (this.checkSignature(xe.CENTRAL_DIRECTORY_END), this.readBlockEndOfCentral(), this.diskNumber === Le.MAX_VALUE_16BITS || this.diskWithCentralDirStart === Le.MAX_VALUE_16BITS || this.centralDirRecordsOnThisDisk === Le.MAX_VALUE_16BITS || this.centralDirRecords === Le.MAX_VALUE_16BITS || this.centralDirSize === Le.MAX_VALUE_32BITS || this.centralDirOffset === Le.MAX_VALUE_32BITS) {
      if (this.zip64 = !0, e = this.reader.lastIndexOfSignature(xe.ZIP64_CENTRAL_DIRECTORY_LOCATOR), e < 0)
        throw new Error("Corrupted zip: can't find the ZIP64 end of central directory locator");
      if (this.reader.setIndex(e), this.checkSignature(xe.ZIP64_CENTRAL_DIRECTORY_LOCATOR), this.readBlockZip64EndOfCentralLocator(), !this.isSignature(this.relativeOffsetEndOfZip64CentralDir, xe.ZIP64_CENTRAL_DIRECTORY_END) && (this.relativeOffsetEndOfZip64CentralDir = this.reader.lastIndexOfSignature(xe.ZIP64_CENTRAL_DIRECTORY_END), this.relativeOffsetEndOfZip64CentralDir < 0))
        throw new Error("Corrupted zip: can't find the ZIP64 end of central directory");
      this.reader.setIndex(this.relativeOffsetEndOfZip64CentralDir), this.checkSignature(xe.ZIP64_CENTRAL_DIRECTORY_END), this.readBlockZip64EndOfCentral();
    }
    var i = this.centralDirOffset + this.centralDirSize;
    this.zip64 && (i += 20, i += 12 + this.zip64EndOfCentralSize);
    var a = t - i;
    if (a > 0)
      this.isSignature(t, xe.CENTRAL_FILE_HEADER) || (this.reader.zero = a);
    else if (a < 0)
      throw new Error("Corrupted zip: missing " + Math.abs(a) + " bytes.");
  },
  prepareReader: function(e) {
    this.reader = xu(e);
  },
  /**
   * Read a zip file and create ZipEntries.
   * @param {String|ArrayBuffer|Uint8Array|Buffer} data the binary string representing a zip file.
   */
  load: function(e) {
    this.prepareReader(e), this.readEndOfCentral(), this.readCentralDir(), this.readLocalFiles();
  }
};
var Su = Jo, wi = Q(), yr = Kt, Cu = xt, Tu = Su, Au = Ha, ka = Rr;
function Ru(e) {
  return new yr.Promise(function(r, t) {
    var i = e.decompressed.getContentWorker().pipe(new Au());
    i.on("error", function(a) {
      t(a);
    }).on("end", function() {
      i.streamInfo.crc32 !== e.decompressed.crc32 ? t(new Error("Corrupted zip : CRC32 mismatch")) : r();
    }).resume();
  });
}
var Ou = function(e, r) {
  var t = this;
  return r = wi.extend(r || {}, {
    base64: !1,
    checkCRC32: !1,
    optimizedBinaryString: !1,
    createFolders: !1,
    decodeFileName: Cu.utf8decode
  }), ka.isNode && ka.isStream(e) ? yr.Promise.reject(new Error("JSZip can't accept a stream when loading a zip file.")) : wi.prepareContent("the loaded zip file", e, !0, r.optimizedBinaryString, r.base64).then(function(i) {
    var a = new Tu(r);
    return a.load(i), a;
  }).then(function(a) {
    var n = [yr.Promise.resolve(a)], o = a.files;
    if (r.checkCRC32)
      for (var s = 0; s < o.length; s++)
        n.push(Ru(o[s]));
    return yr.Promise.all(n);
  }).then(function(a) {
    for (var n = a.shift(), o = n.files, s = 0; s < o.length; s++) {
      var v = o[s], f = v.fileNameStr, l = wi.resolve(v.fileNameStr);
      t.file(l, v.decompressed, {
        binary: !0,
        optimizedBinaryString: !0,
        date: v.date,
        dir: v.dir,
        comment: v.fileCommentStr.length ? v.fileCommentStr : null,
        unixPermissions: v.unixPermissions,
        dosPermissions: v.dosPermissions,
        createFolders: r.createFolders
      }), v.dir || (t.file(l).unsafeOriginalName = f);
    }
    return n.zipComment.length && (t.comment = n.zipComment), t;
  });
};
function me() {
  if (!(this instanceof me))
    return new me();
  if (arguments.length)
    throw new Error("The constructor with parameters has been removed in JSZip 3.0, please check the upgrade guide.");
  this.files = /* @__PURE__ */ Object.create(null), this.comment = null, this.root = "", this.clone = function() {
    var e = new me();
    for (var r in this)
      typeof this[r] != "function" && (e[r] = this[r]);
    return e;
  };
}
me.prototype = nu;
me.prototype.loadAsync = Ou;
me.support = ie;
me.defaults = be;
me.version = "3.10.1";
me.loadAsync = function(e, r) {
  return new me().loadAsync(e, r);
};
me.external = Kt;
var Du = me, Ut = Cr, Qo = Sr, es = parseInt("0777", 8), Iu = yt.mkdirp = yt.mkdirP = yt;
function yt(e, r, t, i) {
  typeof r == "function" ? (t = r, r = {}) : (!r || typeof r != "object") && (r = { mode: r });
  var a = r.mode, n = r.fs || Qo;
  a === void 0 && (a = es), i || (i = null);
  var o = t || /* istanbul ignore next */
  function() {
  };
  e = Ut.resolve(e), n.mkdir(e, a, function(s) {
    if (!s)
      return i = i || e, o(null, i);
    switch (s.code) {
      case "ENOENT":
        if (Ut.dirname(e) === e) return o(s);
        yt(Ut.dirname(e), r, function(v, f) {
          v ? o(v, f) : yt(e, r, o, f);
        });
        break;
      default:
        n.stat(e, function(v, f) {
          v || !f.isDirectory() ? o(s, i) : o(null, i);
        });
        break;
    }
  });
}
yt.sync = function e(r, t, i) {
  (!t || typeof t != "object") && (t = { mode: t });
  var a = t.mode, n = t.fs || Qo;
  a === void 0 && (a = es), i || (i = null), r = Ut.resolve(r);
  try {
    n.mkdirSync(r, a), i = i || r;
  } catch (s) {
    switch (s.code) {
      case "ENOENT":
        i = e(Ut.dirname(r), t, i), e(r, t, i);
        break;
      default:
        var o;
        try {
          o = n.statSync(r);
        } catch {
          throw s;
        }
        if (!o.isDirectory()) throw s;
        break;
    }
  }
  return i;
};
var ts = { exports: {} };
(function() {
  var e, r = null, t = typeof window == "object" ? window : oe, i = !1, a = t.process, n = Array, o = Error, s = 0, v = 1, f = 2, l = "Symbol", w = "iterator", h = "species", d = l + "(" + h + ")", g = "return", m = "_uh", C = "_pt", u = "_st", _ = "Invalid this", y = "Invalid argument", E = `
From previous `, S = "Chaining cycle detected for promise", F = "Uncaught (in promise)", L = "rejectionHandled", B = "unhandledRejection", z, M, D = { e: r }, X = function() {
  }, se = /^.+\/node_modules\/yaku\/.+\n?/mg, $ = ts.exports = function(T) {
    var O = this, N;
    if (!x(O) || O._s !== e)
      throw ue(_);
    if (O._s = f, i && (O[C] = c()), T !== X) {
      if (!b(T))
        throw ue(y);
      N = te(T)(
        U(O, v),
        U(O, s)
      ), N === D && re(O, s, N.e);
    }
  };
  $.default = $, Pe($, {
    /**
     * Appends fulfillment and rejection handlers to the promise,
     * and returns a new promise resolving to the return value of the called handler.
     * @param  {Function} onFulfilled Optional. Called when the Promise is resolved.
     * @param  {Function} onRejected  Optional. Called when the Promise is rejected.
     * @return {Yaku} It will return a new Yaku which will resolve or reject after
     * @example
     * the current Promise.
     * ```js
     * var Promise = require('yaku');
     * var p = Promise.resolve(10);
     *
     * p.then((v) => {
     *     console.log(v);
     * });
     * ```
     */
    then: function(T, O) {
      if (this._s === void 0) throw ue();
      return lt(
        this,
        P($.speciesConstructor(this, $)),
        T,
        O
      );
    },
    /**
     * The `catch()` method returns a Promise and deals with rejected cases only.
     * It behaves the same as calling `Promise.prototype.then(undefined, onRejected)`.
     * @param  {Function} onRejected A Function called when the Promise is rejected.
     * This function has one argument, the rejection reason.
     * @return {Yaku} A Promise that deals with rejected cases only.
     * @example
     * ```js
     * var Promise = require('yaku');
     * var p = Promise.reject(new Error("ERR"));
     *
     * p['catch']((v) => {
     *     console.log(v);
     * });
     * ```
     */
    catch: function(k) {
      return this.then(e, k);
    },
    // The number of current promises that attach to this Yaku instance.
    _pCount: 0,
    // The parent Yaku.
    _pre: r,
    // A unique type flag, it helps different versions of Yaku know each other.
    _Yaku: 1
  }), $.resolve = function(T) {
    return W(T) ? T : Ge(P(this), T);
  }, $.reject = function(T) {
    return re(P(this), s, T);
  }, $.race = function(T) {
    var O = this, N = P(O), V = function(pe) {
      re(N, v, pe);
    }, K = function(pe) {
      re(N, s, pe);
    }, Te = te(Ce)(T, function(pe) {
      O.resolve(pe).then(V, K);
    });
    return Te === D ? O.reject(Te.e) : N;
  }, $.all = function(T) {
    var O = this, N = P(O), V = [], K;
    function Te(pe) {
      re(N, s, pe);
    }
    return K = te(Ce)(T, function(pe, ss) {
      O.resolve(pe).then(function(fs) {
        V[ss] = fs, --K || re(N, v, V);
      }, Te);
    }), K === D ? O.reject(K.e) : (K || re(N, v, []), N);
  }, $.Symbol = t[l] || {}, te(function() {
    Object.defineProperty($, Be(), {
      get: function() {
        return this;
      }
    });
  })(), $.speciesConstructor = function(k, T) {
    var O = k.constructor;
    return O && O[Be()] || T;
  }, $.unhandledRejection = function(k, T) {
    try {
      t.console.error(
        F,
        i ? T.longStack : Ot(k, T)
      );
    } catch {
    }
  }, $.rejectionHandled = X, $.enableLongStackTrace = function() {
    i = !0;
  }, $.nextTick = a ? a.nextTick : function(k) {
    setTimeout(k);
  }, $._Yaku = 1;
  function Be() {
    return $[l][h] || d;
  }
  function Pe(k, T) {
    for (var O in T)
      k.prototype[O] = T[O];
    return k;
  }
  function x(k) {
    return k && typeof k == "object";
  }
  function b(k) {
    return typeof k == "function";
  }
  function R(k, T) {
    return k instanceof T;
  }
  function j(k) {
    return R(k, o);
  }
  function Z(k, T, O) {
    if (!T(k)) throw ue(O);
  }
  function Y() {
    try {
      return z.apply(M, arguments);
    } catch (k) {
      return D.e = k, D;
    }
  }
  function te(k, T) {
    return z = k, M = T, Y;
  }
  function de(k, T) {
    var O = n(k), N = 0;
    function V() {
      for (var K = 0; K < N; )
        T(O[K], O[K + 1]), O[K++] = e, O[K++] = e;
      N = 0, O.length > k && (O.length = k);
    }
    return function(K, Te) {
      O[N++] = K, O[N++] = Te, N === 2 && $.nextTick(V);
    };
  }
  function Ce(k, T) {
    var O, N = 0, V, K, Te;
    if (!k) throw ue(y);
    var pe = k[$[l][w]];
    if (b(pe))
      V = pe.call(k);
    else if (b(k.next))
      V = k;
    else if (R(k, n)) {
      for (O = k.length; N < O; )
        T(k[N], N++);
      return N;
    } else
      throw ue(y);
    for (; !(K = V.next()).done; )
      if (Te = te(T)(K.value, N++), Te === D)
        throw b(V[g]) && V[g](), Te.e;
    return N;
  }
  function ue(k) {
    return new TypeError(k);
  }
  function c(k) {
    return (k ? "" : E) + new o().stack;
  }
  var p = de(999, function(k, T) {
    var O, N;
    if (N = k._s ? T._onFulfilled : T._onRejected, N === e) {
      re(T, k._s, k._v);
      return;
    }
    if (O = te(jr)(N, k._v), O === D) {
      re(T, s, O.e);
      return;
    }
    Ge(T, O);
  }), A = de(9, function(k) {
    ut(k) || (k[m] = 1, I(B, k));
  });
  function I(k, T) {
    var O = "on" + k.toLowerCase(), N = t[O];
    a && a.listeners(k).length ? k === B ? a.emit(k, T._v, T) : a.emit(k, T) : N ? N({ reason: T._v, promise: T }) : $[k](T._v, T);
  }
  function W(k) {
    return k && k._Yaku;
  }
  function P(k) {
    if (W(k)) return new k(X);
    var T, O, N;
    return T = new k(function(V, K) {
      if (T) throw ue();
      O = V, N = K;
    }), Z(O, b), Z(N, b), T;
  }
  function U(k, T) {
    return function(O) {
      i && (k[u] = c(!0)), T === v ? Ge(k, O) : re(k, T, O);
    };
  }
  function lt(k, T, O, N) {
    return b(O) && (T._onFulfilled = O), b(N) && (k[m] && I(L, k), T._onRejected = N), i && (T._pre = k), k[k._pCount++] = T, k._s !== f && p(k, T), T;
  }
  function ut(k) {
    if (k._umark)
      return !0;
    k._umark = !0;
    for (var T = 0, O = k._pCount, N; T < O; )
      if (N = k[T++], N._onRejected || ut(N)) return !0;
  }
  function Ot(k, T) {
    var O = [];
    function N(V) {
      return O.push(V.replace(/^\s+|\s+$/g, ""));
    }
    return i && (T[u] && N(T[u]), function V(K) {
      K && C in K && (V(K._next), N(K[C] + ""), V(K._pre));
    }(T)), (k && k.stack ? k.stack : k) + (`
` + O.join(`
`)).replace(se, "");
  }
  function jr(k, T) {
    return k(T);
  }
  function re(k, T, O) {
    var N = 0, V = k._pCount;
    if (k._s === f)
      for (k._s = T, k._v = O, T === s && (i && j(O) && (O.longStack = Ot(O, k)), A(k)); N < V; )
        p(k, k[N++]);
    return k;
  }
  function Ge(k, T) {
    if (T === k && T)
      return re(k, s, ue(S)), k;
    if (T !== r && (b(T) || x(T))) {
      var O = te(Dt)(T);
      if (O === D)
        return re(k, s, O.e), k;
      b(O) ? (i && W(T) && (k._next = T), W(T) ? ht(k, T, O) : $.nextTick(function() {
        ht(k, T, O);
      })) : re(k, v, T);
    } else
      re(k, v, T);
    return k;
  }
  function Dt(k) {
    return k.then;
  }
  function ht(k, T, O) {
    var N = te(O, T)(function(V) {
      T && (T = r, Ge(k, V));
    }, function(V) {
      T && (T = r, re(k, s, V));
    });
    N === D && T && (re(k, s, N.e), T = r);
  }
})();
var Bu = ts.exports, Fu = Bu, Lu = {
  isFunction: function(e) {
    return typeof e == "function";
  },
  Promise: Fu
}, rs = Lu, vt = rs.isFunction, Nu = function(e, r) {
  return function(t, i, a, n, o) {
    var s = arguments.length, v, f, l, w;
    f = new rs.Promise(function(g, m) {
      l = g, w = m;
    });
    function h(g, m) {
      g == null ? l(m) : w(g);
    }
    switch (s) {
      case 0:
        e.call(r, h);
        break;
      case 1:
        vt(t) ? e.call(r, t) : e.call(r, t, h);
        break;
      case 2:
        vt(i) ? e.call(r, t, i) : e.call(r, t, i, h);
        break;
      case 3:
        vt(a) ? e.call(r, t, i, a) : e.call(r, t, i, a, h);
        break;
      case 4:
        vt(n) ? e.call(r, t, i, a, n) : e.call(r, t, i, a, n, h);
        break;
      case 5:
        vt(o) ? e.call(r, t, i, a, n, o) : e.call(r, t, i, a, n, o, h);
        break;
      default:
        v = new Array(s);
        for (var d = 0; d < s; d++)
          v[d] = arguments[d];
        if (vt(v[s - 1]))
          return e.apply(r, v);
        v[d] = h, e.apply(r, v);
    }
    return f;
  };
}, is = Sr, Ve = Cr, zu = Du, Pu = Iu, Gi = Nu, ju = Gi(is.writeFile), Uu = Gi(is.readFile), Mu = Gi(Pu);
function $u(e) {
  function r(f, l, w, h) {
    var d = 0;
    return d += f, d += l << 8, d += w << 16, d += h << 24, d;
  }
  if (e[0] === 80 && e[1] === 75 && e[2] === 3 && e[3] === 4)
    return e;
  if (e[0] !== 67 || e[1] !== 114 || e[2] !== 50 || e[3] !== 52)
    throw new Error("Invalid header: Does not start with Cr24");
  var t = e[4] === 3, i = e[4] === 2;
  if (!i && !t || e[5] || e[6] || e[7])
    throw new Error("Unexpected crx format version number.");
  if (i) {
    var a = r(e[8], e[9], e[10], e[11]), n = r(e[12], e[13], e[14], e[15]), o = 16 + a + n;
    return e.slice(o, e.length);
  }
  var s = r(e[8], e[9], e[10], e[11]), v = 12 + s;
  return e.slice(v, e.length);
}
function Zu(e, r) {
  var t = Ve.resolve(e), i = Ve.extname(e), a = Ve.basename(e, i), n = Ve.dirname(e);
  return r = r || Ve.resolve(n, a), Uu(t).then(function(o) {
    return zu.loadAsync($u(o));
  }).then(function(o) {
    var s = Object.keys(o.files);
    return Promise.all(s.map(function(v) {
      var f = !o.files[v].dir, l = Ve.join(r, v), w = f && Ve.dirname(l) || l, h = o.files[v].async("nodebuffer");
      return Mu(w).then(function() {
        return f ? h : !1;
      }).then(function(d) {
        return d ? ju(l, d) : !0;
      });
    }));
  });
}
var Wu = Zu;
(function(e) {
  Object.defineProperty(e, "__esModule", { value: !0 }), e.downloadChromeExtension = void 0;
  const r = Sr, t = Cr, i = Aa, a = Wu, n = async (o, { forceDownload: s = !1, attempts: v = 5 } = {}) => {
    const f = (0, i.getPath)();
    r.existsSync(f) || await r.promises.mkdir(f, { recursive: !0 });
    const l = t.resolve(`${f}/${o}`);
    if (!r.existsSync(l) || s) {
      r.existsSync(l) && await r.promises.rmdir(l, {
        recursive: !0
      });
      const w = `https://clients2.google.com/service/update2/crx?response=redirect&acceptformat=crx2,crx3&x=id%3D${o}%26uc&prodversion=${process.versions.chrome}`, h = t.resolve(`${l}.crx`);
      try {
        await (0, i.downloadFile)(w, h);
        try {
          return await a(h, l), (0, i.changePermissions)(l, 755), l;
        } catch (d) {
          if (!r.existsSync(t.resolve(l, "manifest.json")))
            throw d;
        }
      } catch (d) {
        if (console.error(`Failed to fetch extension, trying ${v - 1} more times`), v <= 1)
          throw d;
        return await new Promise((g) => setTimeout(g, 200)), await (0, e.downloadChromeExtension)(o, {
          forceDownload: s,
          attempts: v - 1
        });
      }
    }
    return l;
  };
  e.downloadChromeExtension = n;
})(Ta);
Object.defineProperty(ne, "__esModule", { value: !0 });
ne.MOBX_DEVTOOLS = ne.REDUX_DEVTOOLS = ne.VUEJS_DEVTOOLS_BETA = ne.VUEJS_DEVTOOLS = ne.JQUERY_DEBUGGER = ne.BACKBONE_DEBUGGER = ns = ne.REACT_DEVELOPER_TOOLS = ne.EMBER_INSPECTOR = void 0;
ne.installExtension = Vi;
const Hu = Ea, qu = Ta;
async function Vi(e, r = {}) {
  const { forceDownload: t, loadExtensionOptions: i, session: a } = r, n = a || Hu.session.defaultSession;
  if (process.type !== "browser")
    return Promise.reject(new Error("electron-devtools-installer can only be used from the main process"));
  if (Array.isArray(e))
    return e.reduce((f, l) => f.then(async (w) => {
      const h = await Vi(l, r);
      return [...w, h];
    }), Promise.resolve([]));
  let o;
  if (typeof e == "object" && e.id)
    o = e.id;
  else if (typeof e == "string")
    o = e;
  else
    throw new Error(`Invalid extensionReference passed in: "${e}"`);
  const s = n.getAllExtensions().find((f) => f.id === o);
  if (!t && s)
    return s;
  const v = await (0, qu.downloadChromeExtension)(o, {
    forceDownload: t || !1
  });
  if (s != null && s.id) {
    const f = new Promise((l) => {
      const w = (h, d) => {
        d.id === s.id && (n.removeListener("extension-unloaded", w), l());
      };
      n.on("extension-unloaded", w);
    });
    n.removeExtension(s.id), await f;
  }
  return n.loadExtension(v, i);
}
var Yu = ne.default = Vi;
ne.EMBER_INSPECTOR = {
  id: "bmdblncegkenkacieihfhpjfppoconhi"
};
var ns = ne.REACT_DEVELOPER_TOOLS = {
  id: "fmkadmapgofadopljbjfkapdkoienihi"
};
ne.BACKBONE_DEBUGGER = {
  id: "bhljhndlimiafopmmhjlgfpnnchjjbhd"
};
ne.JQUERY_DEBUGGER = {
  id: "dbhhnnnpaeobfddmlalhnehgclcmjimi"
};
ne.VUEJS_DEVTOOLS = {
  id: "nhdogjmejiglipccpnnnanhbledajbpd"
};
ne.VUEJS_DEVTOOLS_BETA = {
  id: "ljjemllljcmogpfapbkkighbhhppjdbg"
};
ne.REDUX_DEVTOOLS = {
  id: "lmhkpmbekcpmknklioeibfkpmmfibljd"
};
ne.MOBX_DEVTOOLS = {
  id: "pfgnfdagidkfgccljigdamigbcnndkod"
};
const Ku = et.dirname(ls(import.meta.url));
process.env.APP_ROOT = et.join(Ku, "..");
const Oi = process.env.VITE_DEV_SERVER_URL, ah = et.join(process.env.APP_ROOT, "dist-electron"), as = et.join(process.env.APP_ROOT, "dist");
process.env.VITE_PUBLIC = Oi ? et.join(process.env.APP_ROOT, "public") : as;
let Ze;
function os() {
  Ze = new Sa({
    icon: et.join(process.env.VITE_PUBLIC, "electron-vite.svg"),
    webPreferences: {
      // preload: path.join(__dirname, 'preload.mjs'), // Disabled preload script due to contextIsolation conflict
      // WARNING: These settings are generally NOT recommended for production Electron applications
      // due to significant security risks. They are included here for compatibility with the
      // original application's behavior (which relied on Node.js APIs directly in the renderer).
      // For a secure production app, `contextIsolation` should be `true` and `nodeIntegration` `false`,
      // with necessary APIs exposed securely via the preload script using `contextBridge`.
      nodeIntegration: !0,
      contextIsolation: !1
    }
  }), Ze.webContents.on("did-finish-load", () => {
    Ze == null || Ze.webContents.send("main-process-message", (/* @__PURE__ */ new Date()).toLocaleString());
  }), Oi ? Ze.loadURL(Oi) : Ze.loadFile(et.join(as, "index.html"));
}
br.on("window-all-closed", () => {
  process.platform !== "darwin" && (br.quit(), Ze = null);
});
br.on("activate", () => {
  Sa.getAllWindows().length === 0 && os();
});
br.whenReady().then(() => {
  Yu(ns).then((e) => console.log(`Added Extension: ${e}`)).catch((e) => console.log("An error occurred installing React DevTools: ", e)), os();
});
export {
  ah as MAIN_DIST,
  as as RENDERER_DIST,
  Oi as VITE_DEV_SERVER_URL
};
