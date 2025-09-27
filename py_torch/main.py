import torch
x = torch.randn(3, requires_grad=False)
print(x)

y = x + 2
print(y)

z = y * y * 2
print(z)
z = z.mean()
print(z)

# z.backward()
# print(x.grad)