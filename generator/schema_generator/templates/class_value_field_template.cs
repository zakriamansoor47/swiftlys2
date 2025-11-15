  private static readonly Lazy<nint> _$NAME$Offset = new(() => Schema.GetOffset($HASH$), LazyThreadSafetyMode.None);

  public ref $IMPL_TYPE$ $NAME$ {
    get => ref _Handle.$REF_METHOD$<$IMPL_TYPE$>(_$NAME$Offset.Value);
  }