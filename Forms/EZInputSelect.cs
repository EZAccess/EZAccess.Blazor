// Licensed to the.NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Rendering;

namespace EZAccess.Blazor.Forms;

/// <summary>
/// A dropdown selection component.
/// </summary>
public class EZInputSelect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue?>
{
    [CascadingParameter(Name = "ActionOnFocus")] private Action<bool>? ActionOnFocus { get; set; }
    [Parameter] public string? Id { get; set; }
    [Parameter] public bool Editable { get; set; }
    [Parameter] public Expression<Func<TValue?>>? ValueExpressionOverwrite { get; set; }

    /// <summary>
    /// Overwrite the parameters to get the correct ValueExpression, which list used to determine the FieldIdentifier
    /// </summary>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        if (ValueExpressionOverwrite != null)
        {
            ValueExpression = ValueExpressionOverwrite;
        }
        return base.SetParametersAsync(ParameterView.Empty);
    }

    private readonly bool _isMultipleSelect;

    /// <summary>
    /// Constructs an instance of <see cref="InputSelect{TValue}"/>.
    /// </summary>
    public EZInputSelect()
    {
        _isMultipleSelect = typeof(TValue).IsArray;
    }

    /// <summary>
    /// Gets or sets the child content to be rendering inside the select element.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the <c>select</c> <see cref="ElementReference"/>.
    /// <para>
    /// May be <see langword="null"/> if accessed before the component is rendered.
    /// </para>
    /// </summary>
    [DisallowNull] public ElementReference? Element { get; protected set; }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "select");
        builder.AddAttribute(2, "autocomplete", "off");
        if (!Editable)
        {
            builder.AddAttribute(3, "readonly");
        }
        builder.AddMultipleAttributes(4, AdditionalAttributes);
        builder.AddAttributeIfNotNullOrEmpty(5, "class", CssClass);
        builder.AddAttribute(6, "multiple", _isMultipleSelect);

        if (_isMultipleSelect)
        {
            builder.AddAttribute(7, "value", BindConverter.FormatValue(CurrentValue)?.ToString());
            builder.AddAttribute(8, "onchange", EventCallback.Factory.CreateBinder<string?[]?>(this, SetCurrentValueAsStringArray, default));
        }
        else
        {
            builder.AddAttribute(7, "value", CurrentValueAsString);
            builder.AddAttribute(8, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, default));
        }

        builder.AddAttribute(9, "onfocus", EventCallback.Factory.Create(this, OnFocus));
        builder.AddElementReferenceCapture(10, __selectReference => Element = __selectReference);
        builder.AddContent(11, ChildContent);
        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        return this.TryParseSelectableValueFromString(value, out result, out validationErrorMessage, FieldIdentifier.FieldName);
    }

    /// <inheritdoc />
    protected override string? FormatValueAsString(TValue? value)
    {
        // We special-case bool values because BindConverter reserves bool conversion for conditional attributes.
        if (typeof(TValue) == typeof(bool))
        {
            return (bool)(object)value! ? "true" : "false";
        }
        else if (typeof(TValue) == typeof(bool?))
        {
            return value is not null && (bool)(object)value ? "true" : "false";
        }

        return base.FormatValueAsString(value);
    }

    private void SetCurrentValueAsStringArray(string?[]? value)
    {
        CurrentValue = BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var result)
            ? result
            : default;
    }

    private void OnFocus()
    {
        ActionOnFocus?.Invoke(false);
    }

}