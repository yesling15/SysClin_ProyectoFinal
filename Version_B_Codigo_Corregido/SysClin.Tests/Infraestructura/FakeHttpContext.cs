using SysClin.Models;
using System.Collections.Generic;
using System.Web;

public class FakeHttpContext : HttpContextBase
{
    private readonly IDictionary<string, object> _session;

    public FakeHttpContext()
    {
        _session = new Dictionary<string, object>();
    }

    public override HttpSessionStateBase Session =>
        new FakeHttpSession(_session);
}

public class FakeHttpSession : HttpSessionStateBase
{
    private readonly IDictionary<string, object> _storage;

    public FakeHttpSession(IDictionary<string, object> storage)
    {
        _storage = storage;
    }

    public override object this[string name]
    {
        get => _storage.ContainsKey(name) ? _storage[name] : null;
        set => _storage[name] = value;
    }
}