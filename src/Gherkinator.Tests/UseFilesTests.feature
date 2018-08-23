Feature: UseFiles
  An extension that allows assigning and verifying file contents

  Scenario: Setting file contents inline
    Given Foo.txt = Bar
    Then Foo.txt = "Bar"

  Scenario: Setting file contents block
    Given Foo.txt = 
"""
Bar
"""
    Then Foo.txt = Bar

  Scenario: Verifying file contents
    Given Foo.txt = Bar
    Then Foo.txt = Baz

  Scenario: Disposing deletes temporary directory
    Given Foo.txt = Bar
    Then Foo.txt = Bar