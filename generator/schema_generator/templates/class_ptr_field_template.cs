  private static readonly Lazy<nint> _$NAME$Offset = new(() => Schema.GetOffset($HASH$), LazyThreadSafetyMode.None);

  public $INTERFACE_TYPE$? $NAME$ {
    get {
      var ptr = _Handle.Read<nint>(_$NAME$Offset.Value);
      return ptr.IsValidPtr() ? new $IMPL_TYPE$(ptr) : null;
    }
  }